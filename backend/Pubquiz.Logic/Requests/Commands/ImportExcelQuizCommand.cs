using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.Extensions.Logging;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Rebus.Bus;

namespace Pubquiz.Logic.Requests.Commands
{
    public class ImportZippedExcelQuizCommand : Command<string>
    {
        private readonly Stream _fileStream;
        private readonly string _fileName;
        private readonly QuizrSettings _quizrSettings;
        private QuizrPackage _package;
        private string _packagePath;
        private readonly ILogger<ImportZippedExcelQuizCommand> _logger;

        public ImportZippedExcelQuizCommand(IUnitOfWork unitOfWork, IBus bus, Stream fileStream, string fileName,
            QuizrSettings quizrSettings, ILoggerFactory loggerFactory) :
            base(unitOfWork, bus)
        {
            _fileStream = fileStream;
            _fileName = fileName;
            _quizrSettings = quizrSettings;
            _logger = loggerFactory.CreateLogger<ImportZippedExcelQuizCommand>();
        }

        protected override async Task<string> DoExecute()
        {
            if (_fileStream == null)
            {
                throw new DomainException("No file uploaded.", true);
            }

            var filePath = Path.Combine(_quizrSettings.ContentPath, _fileName);
            await using (var fileStream = File.OpenWrite(filePath))
            {
                await _fileStream.CopyToAsync(fileStream);
                _fileStream.Position = 0;
            }

            var isValidQuizrPackage = false;
            try
            {
                using var zipFile = ZipFile.OpenRead(filePath);
                isValidQuizrPackage = zipFile.Entries.Any(e => e.Name.EndsWith(".xlsx"));
            }
            catch
            {
                // Intentionally unhandled, isValidQtiPackage is already false;
            }

            if (!isValidQuizrPackage)
            {
                File.Delete(filePath);
                throw new DomainException("Het geüploade bestand is geen geldige Quizr-package.", true);
            }

            using var md5 = MD5.Create();
            var hashBytes = md5.ComputeHash(_fileStream);
            var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

            var packageCollection = UnitOfWork.GetCollection<QuizrPackage>();
            _package = packageCollection.AsQueryable().FirstOrDefault(p => p.Hash == hash);
            if (_package == null)
            {
                _package = new QuizrPackage
                {
                    Hash = hash,
                    FullPath = filePath,
                    Filename = _fileName
                };
                await packageCollection.AddAsync(_package);
            }

            await Unzip();

            await ImportExcel();
            await packageCollection.UpdateAsync(_package);

            return _package.Id;
        }

        private async Task ImportExcel()
        {
            var dirInfo = new DirectoryInfo(_packagePath);
            var excelFiles = dirInfo.GetFiles("*.xlsx");
            var quizzes = new List<Quiz>();
            foreach (var excelFile in excelFiles)
            {
                var errors = new List<string>();
                var quiz = new Quiz();
                await using var stream = excelFile.OpenRead();
                using var reader = ExcelReaderFactory.CreateReader(stream);
                var dataSet = reader.AsDataSet();
                var quizSheet = dataSet.Tables["Quiz"];
                ParseSettings(quizSheet, quiz, errors);
                if (errors.Any())
                {
                    throw new ImportException(errors, "Errors while parsing the quiz sheet.");
                }

                var quizItemsSheet = dataSet.Tables["QuizItems"];
                ParseQuizItems(quizItemsSheet, quiz, errors);
                if (errors.Any())
                {
                    throw new ImportException(errors, "Errors while parsing the quiz items sheet.");
                }

                quizzes.Add(quiz);
            }

            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            foreach (var quiz in quizzes)
            {
                await quizCollection.AddAsync(quiz);
                _package.QuizIds.Add(quiz.Id);
            }
        }

        private void ParseQuizItems(DataTable quizItemsSheet, Quiz quiz, List<string> errors)
        {
            var enumerableRows = quizItemsSheet.AsEnumerable();

            // check column headers
            var headerRow = enumerableRows.First();
            if (headerRow.Field<string>(0) != "SectionTitle" ||
                headerRow.Field<string>(1) != "QuizItemTitle" ||
                headerRow.Field<string>(2) != "QuizItemType" ||
                headerRow.Field<string>(3) != "Body" ||
                headerRow.Field<string>(4) != "QuizItemMaxScore" ||
                headerRow.Field<string>(5) != "MediaTitle" ||
                headerRow.Field<string>(6) != "MediaType" ||
                headerRow.Field<string>(7) != "MediaFileName" ||
                headerRow.Field<string>(8) != "MediaUrl" ||
                headerRow.Field<string>(9) != "InteractionType" ||
                headerRow.Field<string>(10) != "Text" ||
                headerRow.Field<string>(11) != "InteractionMaxScore" ||
                headerRow.Field<string>(12) != "Solutions" ||
                headerRow.Field<string>(13) != "Choices" ||
                headerRow.Field<string>(14) != "LevenshteinTolerance" ||
                headerRow.Field<string>(15) != "FlagIfWithinTolerance") 
            {
                errors.Add("The column headers are incorrect.");
                return;
            }

            var quizItemRows = enumerableRows.Skip(1);

            var quizItemCounter = 0;
            var rowCounter = 1;
            var quizSectionCounter = 0;
            var quizItems = new List<QuizItem>();
            QuizSection currentQuizSection = null;
            QuizItem currentQuizItem = null;
            QuizItemRef currentQuizItemRef = null;
            foreach (var quizItemRow in quizItemRows)
            {
                rowCounter++;
                
                // quiz section
                try
                {
                    var quizSectionTitle = quizItemRow[0].ToString().Trim();
                    if (!quiz.QuizSections.Exists(s => s.Title == quizSectionTitle)) // new quiz section
                    {
                        quizSectionCounter++;
                        currentQuizSection = new QuizSection {Title = quizSectionTitle};
                        quiz.QuizSections.Add(currentQuizSection);
                    }
                }
                catch (InvalidCastException)
                {
                    errors.Add($"The SectionTitle is incorrectly specified for item on row {rowCounter}");
                }

                if (currentQuizSection == null)
                {
                    throw new DomainException($"Missing SectionTitle on row {rowCounter}", true);
                }

                // QuizItemTitle
                try
                {
                    var quizItemTitle = quizItemRow[1].ToString().Trim();
                    if (!currentQuizSection.QuizItemRefs.Exists(i => i.Title == quizItemTitle)) // new quiz section
                    {
                        quizItemCounter++;
                        currentQuizItem = new QuizItem
                        {
                            Title = quizItemTitle
                        };
                        quizItems.Add(currentQuizItem);

                        currentQuizItemRef = new QuizItemRef
                        {
                            Id = currentQuizItem.Id,
                            Title = currentQuizItem.Title
                        };
                        currentQuizSection.QuizItemRefs.Add(currentQuizItemRef);
                    }
                }
                catch (InvalidCastException)
                {
                    errors.Add($"The QuizItemTitle is incorrectly specified for item on row {rowCounter}");
                }

                if (currentQuizItem == null || currentQuizItemRef == null)
                {
                    throw new DomainException($"Missing QuizItemTitle on row {rowCounter}", true);
                }

                // QuizItemType
                try
                {
                    var quizItemTypeString = quizItemRow[2].ToString().Trim();
                    var quizItemType = Enum.Parse<QuizItemType>(quizItemTypeString, true);
                    currentQuizItem.QuizItemType = quizItemType;
                    currentQuizItemRef.ItemType = quizItemType;
                }
                catch (InvalidCastException)
                {
                    errors.Add($"The QuizItemType is incorrectly specified for item on row {rowCounter}");
                }
                
                // Body
                try
                {
                    var body = quizItemRow[3].ToString().Trim();
                    currentQuizItem.Body = body;
                }
                catch (InvalidCastException)
                {
                    errors.Add($"The QuizItemType is incorrectly specified for item on row {rowCounter}");
                }

                if (currentQuizItem.QuizItemType!=QuizItemType.Information)
                {
                    // QuizItemMaxScore
                    try
                    {
                        var quizItemMaxScore = Convert.ToInt32(quizItemRow.Field<double>(4));
                        currentQuizItem.MaxScore = quizItemMaxScore;
                    }
                    catch (InvalidCastException)
                    {
                        errors.Add($"The QuizItemMaxScore is incorrectly specified for item on row {rowCounter}");
                    }
                }
            
                
                // MediaTitle 5
                // MediaType 6
                // MediaFileName 7
                // MediaUrl 8
                
                // InteractionType 9
                // Text 10
                // InteractionMaxScore 11
                // Solutions 12
                // Choices 13
                // LevenshteinTolerance 14
                // FlagIfWithinTolerance 15

            }
        }

        private void ParseSettings(DataTable startSheet, Quiz quiz, List<string> errors)
        {
            var enumerableRows = startSheet.AsEnumerable();
            // Quiz name
            var nameRow = enumerableRows.FirstOrDefault(r => r.Field<string>(0) == "QuizTitle");
            if (nameRow == null)
            {
                errors.Add("Setting not found: 'QuizTitle'");
            }
            else
            {
                quiz.Title = nameRow.Field<string>(1).Trim();
                if (string.IsNullOrWhiteSpace(quiz.Title))
                {
                    errors.Add("Setting is empty: 'QuizTitle'");
                }
            }
        }

        private async Task Unzip()
        {
            // unzip the package
            _packagePath = Path.Combine(_quizrSettings.ContentPath, _package.Id);
            _logger.LogInformation($"Extracting to {_packagePath}");
            await Task.Run(() => ZipFile.ExtractToDirectory(_package.FullPath, _packagePath, true));
        }
    }
}