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
using Pubquiz.Domain.ViewModels;
using Pubquiz.Persistence;

// ReSharper disable TemplateIsNotCompileTimeConstantProblem

namespace Pubquiz.Logic.Tools
{
    public class ExcelQuizImporter
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ExcelQuizImporter> _logger;
        private readonly QuizrSettings _quizrSettings;
        private readonly Stream _fileStream;
        private readonly string _fileName;
        private readonly string _actorId;

        private QuizrPackage _package;
        private string _packagePath;

        public ExcelQuizImporter(IUnitOfWork unitOfWork, ILoggerFactory loggerFactory, QuizrSettings quizrSettings,
            Stream fileStream, string fileName, string actorId)
        {
            _unitOfWork = unitOfWork;
            _quizrSettings = quizrSettings;
            _fileStream = fileStream;
            _fileName = fileName;
            _actorId = actorId;
            _logger = loggerFactory.CreateLogger<ExcelQuizImporter>();
        }

        public async Task<QuizrPackage> Import()
        {
            if (_fileStream == null)
            {
                throw new DomainException("No file uploaded.", true);
            }

            using var md5 = MD5.Create();
            var hashBytes = md5.ComputeHash(_fileStream);
            var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            _fileStream.Position = 0;
            var filePath = Path.Combine(_quizrSettings.WebRootPath, _quizrSettings.ContentPath, _fileName);
            await using (var fileStream = File.OpenWrite(filePath))
            {
                await _fileStream.CopyToAsync(fileStream);
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
                throw new DomainException(ResultCode.ValidationError,"Het geüploade bestand is geen geldige Quizr-package.", true);
            }

            var packageCollection = _unitOfWork.GetCollection<QuizrPackage>();
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
            else
            {
                File.Delete(filePath);
                _logger.LogInformation($"Package with hash {hash} was previously imported, returning error.");
                throw new DomainException(ResultCode.ValidationError, "Het geüploade bestand bestaat al in het systeem.", true);
            }

            await Unzip();

            await ImportExcel();
            await packageCollection.UpdateAsync(_package);

            return _package;
        }

        private async Task ImportExcel()
        {
            var dirInfo = new DirectoryInfo(_packagePath);
            var excelFiles = dirInfo.GetFiles("*.xlsx");
            var quizzes = new List<Quiz>();
            var quizItemList = new List<QuizItem>();
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
                var quizItems = new List<QuizItem>();
                ParseQuizItems(quizItemsSheet, quiz, quizItems, errors);
                if (errors.Any())
                {
                    throw new ImportException(errors,
                        $"Errors while parsing the quiz items sheet. {string.Join(", ", errors)}");
                }

                quizzes.Add(quiz);
                quizItemList.AddRange(quizItems);
            }

            var quizCollection = _unitOfWork.GetCollection<Quiz>();
            var userCollection = _unitOfWork.GetCollection<User>();
            var user = await userCollection.GetAsync(_actorId);

            foreach (var quiz in quizzes)
            {
                await quizCollection.AddAsync(quiz);
                var quizVm = new QmQuizViewModel {Id = quiz.Id, Title = quiz.Title};
                _package.QuizViewModels.Add(quizVm);
                user.QuizIds.Add(quiz.Id);
            }

            await userCollection.UpdateAsync(user);

            var quizItemCollection = _unitOfWork.GetCollection<QuizItem>();
            foreach (var quizItem in quizItemList)
            {
                await quizItemCollection.AddAsync(quizItem);
            }
        }

        private void ParseQuizItems(DataTable quizItemsSheet, Quiz quiz, List<QuizItem> quizItems, List<string> errors)
        {
            var enumerableRows = quizItemsSheet.AsEnumerable();

            if (CheckColumnHeaders(errors, enumerableRows)) return;

            var quizItemRows = enumerableRows.Skip(1);

            var quizItemCounter = 0;
            var rowCounter = 1;
            var quizSectionCounter = 0;
            QuizSection currentQuizSection = null;
            QuizItem currentQuizItem = null;
            QuizItemRef currentQuizItemRef = null;
            foreach (var quizItemRow in quizItemRows)
            {
                rowCounter++;

                // skip row if completely empty
                if (quizItemRow.IsEmpty())
                {
                    continue;
                }

                // quiz section 0
                var quizSectionTitle = quizItemRow[0].ToString().Trim();
                if (!string.IsNullOrWhiteSpace(quizSectionTitle) &&
                    !quiz.QuizSections.Exists(s => s.Title == quizSectionTitle)) // new quiz section
                {
                    quizSectionCounter++;
                    currentQuizSection = new QuizSection {Title = quizSectionTitle};
                    quiz.QuizSections.Add(currentQuizSection);
                }

                if (currentQuizSection == null)
                {
                    throw new DomainException($"Missing SectionTitle on row {rowCounter}", true);
                }

                // QuizItemTitle 1

                var quizItemTitle = quizItemRow[1].ToString().Trim();
                bool continuedQuizItem;
                if (!string.IsNullOrWhiteSpace(quizItemTitle) &&
                    !currentQuizSection.QuizItemRefs.Exists(i => i.Title == quizItemTitle)) // new quiz section
                {
                    continuedQuizItem = false;
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
                else
                {
                    continuedQuizItem = true;
                }

                if (currentQuizItem == null || currentQuizItemRef == null)
                {
                    throw new DomainException($"Missing QuizItemTitle on row {rowCounter}", true);
                }

                // QuizItemType 2
                if (!continuedQuizItem)
                {
                    var quizItemTypeString = quizItemRow[2].ToString().Trim();
                    if (Enum.TryParse<QuizItemType>(quizItemTypeString, true, out var quizItemType))
                    {
                        currentQuizItem.QuizItemType = quizItemType;
                        currentQuizItemRef.ItemType = quizItemType;
                    }
                    else
                    {
                        errors.Add($"The QuizItemType is incorrectly specified for item on row {rowCounter}");
                    }

                    // Body 3
                    var body = quizItemRow[3].ToString().Trim();
                    if (!string.IsNullOrWhiteSpace(body))
                    {
                        currentQuizItem.Body = body;
                    }

                    if (currentQuizItem.QuizItemType != QuizItemType.Information)
                    {
                        // QuizItemMaxScore 4
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
                }

                // MediaType 5
                var mediaObjectErrors = 0;
                var mediaTypeString = quizItemRow[5].ToString().Trim();
                if (!string.IsNullOrWhiteSpace(mediaTypeString))
                {
                    var mediaObject = new MediaObject();

                    if (Enum.TryParse<MediaType>(mediaTypeString, true, out var mediaType))
                    {
                        mediaObject.MediaType = mediaType;
                    }
                    else
                    {
                        errors.Add($"Missing MediaType on row {rowCounter}");
                        mediaObjectErrors++;
                    }

                    // MediaTitle 6
                    var mediaTitle = quizItemRow[6].ToString().Trim();
                    mediaObject.Title = mediaTitle;

                    // MediaSource 7
                    var mediaSource = quizItemRow[7].ToString().Trim();
                    if (string.IsNullOrWhiteSpace(mediaSource))
                    {
                        // MediaUrl 8
                        var mediaUrl = quizItemRow[8].ToString().Trim();
                        if (string.IsNullOrWhiteSpace(mediaUrl))
                        {
                            errors.Add(
                                $"Missing MediaFileName and MediaUrl on row {rowCounter} (one of both needs to be specified).");
                            mediaObjectErrors++;
                        }
                    }
                    else
                    {
                        if (mediaObject.MediaType == MediaType.Markdown)
                        {
                            mediaObject.Text = mediaSource;
                        }
                        else
                        {
                            mediaObject.Uri =
                                $"{_quizrSettings.BaseUrl}/{_quizrSettings.ContentPath}/{_package.Hash}/{mediaSource}";
                        }
                    }

                    // MediaIsSolution 9
                    var mediaIsSolutionString = quizItemRow[9].ToString().Trim().ToUpperInvariant();
                    if (!string.IsNullOrWhiteSpace(mediaIsSolutionString))
                    {
                        switch (mediaIsSolutionString)
                        {
                            case "YES":
                            case "TRUE":
                                mediaObject.IsSolution = true;
                                break;
                            case "NO":
                            case "FALSE":
                                mediaObject.IsSolution = false;
                                break;
                            default:
                                errors.Add(
                                    $"Invalid MediaIsSolution '{mediaIsSolutionString}' on row {rowCounter}");
                                mediaObjectErrors++;
                                break;
                        }
                    }
                    else
                    {
                        errors.Add(
                            $"The MediaIsSolution is incorrectly specified for item on row {rowCounter}");
                        mediaObjectErrors++;
                    }

                    if (mediaObjectErrors == 0)
                    {
                        currentQuizItem.MediaObjects.Add(mediaObject);
                    }
                }

                var interactionErrors = 0;
                var interactionTypeString = quizItemRow[10].ToString().Trim();
                if (!string.IsNullOrWhiteSpace(interactionTypeString))
                {
                    var interaction = new Interaction(currentQuizItem.Interactions.Count);
                    // InteractionType 10
                    if (Enum.TryParse<InteractionType>(interactionTypeString, true, out var interactionType))
                    {
                        interaction.InteractionType = interactionType;
                    }
                    else
                    {
                        errors.Add($"Invalid InteractionType on row {rowCounter}");
                        interactionErrors++;
                    }

                    // Text 11
                    var interactionText = quizItemRow[11].ToString().Trim();
                    if (!string.IsNullOrWhiteSpace(interactionText))
                    {
                        interaction.Text = interactionText;
                    }
                    else
                    {
                        errors.Add($"Missing InteractionText on row {rowCounter}");
                        interactionErrors++;
                    }

                    // InteractionMaxScore 12
                    try
                    {
                        var interactionMaxScore = Convert.ToInt32(quizItemRow.Field<double>(12));
                        interaction.MaxScore = interactionMaxScore;
                    }
                    catch (InvalidCastException)
                    {
                        errors.Add($"The InteractionMaxScore is incorrectly specified for item on row {rowCounter}");
                        interactionErrors++;
                    }

                    // Choices 13
                    if (interaction.InteractionType == InteractionType.MultipleChoice ||
                        interaction.InteractionType == InteractionType.MultipleResponse)
                    {
                        var choicesString = quizItemRow[13].ToString().Trim();
                        if (!string.IsNullOrWhiteSpace(choicesString))
                        {
                            var choicesTokenized = choicesString.Split("||", StringSplitOptions.RemoveEmptyEntries);
                            if (!choicesTokenized.Any())
                            {
                                errors.Add($"No Choices on row {rowCounter}");
                                interactionErrors++;
                            }
                            else
                            {
                                interaction.ChoiceOptions =
                                    choicesTokenized.Select((c, i) => new ChoiceOption {Id = i, Text = c}).ToList();
                            }
                        }
                        else
                        {
                            errors.Add($"Empty Choices on row {rowCounter}");
                            interactionErrors++;
                        }
                    }

                    // Solutions 14
                    var solutionsString = quizItemRow[14].ToString().Trim();
                    if (!string.IsNullOrWhiteSpace(solutionsString))
                    {
                        var solutionsTokenized = solutionsString.Split("&&", StringSplitOptions.RemoveEmptyEntries);
                        if (!solutionsTokenized.Any())
                        {
                            errors.Add($"Invalid Solutions on row {rowCounter}");
                            interactionErrors++;
                        }
                        else
                        {
                            if (interaction.InteractionType == InteractionType.MultipleChoice ||
                                interaction.InteractionType == InteractionType.MultipleResponse)
                            {
                                var choiceOptionIds = new List<int>();
                                foreach (var t in solutionsTokenized)
                                {
                                    var choiceOptionId = int.Parse(t) - 1;
                                    if (choiceOptionId < 0 || choiceOptionId > interaction.ChoiceOptions.Count)
                                    {
                                        errors.Add(
                                            $"Invalid Solution '{choiceOptionId + 1}' specified on row {rowCounter} (out of bounds)");
                                        interactionErrors++;
                                    }

                                    choiceOptionIds.Add(choiceOptionId);
                                }

                                interaction.Solution = new Solution {ChoiceOptionIds = choiceOptionIds};
                            }
                            else
                            {
                                interaction.Solution = new Solution
                                {
                                    Responses = solutionsTokenized.ToList(),
                                };
                            }
                        }
                    }
                    else
                    {
                        errors.Add($"Invalid Solutions on row {rowCounter}");
                        interactionErrors++;
                    }

                    // LevenshteinTolerance 15
                    if (interaction.InteractionType == InteractionType.ShortAnswer)
                    {
                        try
                        {
                            var levenshteinTolerance = Convert.ToInt32(quizItemRow.Field<double>(15));
                            interaction.Solution.LevenshteinTolerance = levenshteinTolerance;
                        }
                        catch (InvalidCastException)
                        {
                            errors.Add(
                                $"The LevenshteinTolerance is incorrectly specified for item on row {rowCounter}");
                            interactionErrors++;
                        }

                        // FlagIfWithinTolerance 16
                        var flagIfWithinToleranceString = quizItemRow[16].ToString().Trim().ToUpperInvariant();
                        if (!string.IsNullOrWhiteSpace(flagIfWithinToleranceString))
                        {
                            switch (flagIfWithinToleranceString)
                            {
                                case "YES":
                                case "TRUE":
                                    interaction.Solution.FlagIfWithinTolerance = true;
                                    break;
                                case "NO":
                                case "FALSE":
                                    interaction.Solution.FlagIfWithinTolerance = false;
                                    break;
                                default:
                                    errors.Add(
                                        $"Invalid FlagIfWithinTolerance '{flagIfWithinToleranceString}' on row {rowCounter}");
                                    interactionErrors++;
                                    break;
                            }
                        }
                        else
                        {
                            errors.Add(
                                $"The FlagIfWithinTolerance is incorrectly specified for item on row {rowCounter}");
                            interactionErrors++;
                        }
                    }

                    if (interactionErrors == 0)
                    {
                        currentQuizItem.Interactions.Add(interaction);
                    }
                }
            }
        }

        private static bool CheckColumnHeaders(List<string> errors, EnumerableRowCollection<DataRow> enumerableRows)
        {
            // check column headers
            var headerRow = enumerableRows.First();
            if (headerRow.Field<string>(0) != "SectionTitle" ||
                headerRow.Field<string>(1) != "QuizItemTitle" ||
                headerRow.Field<string>(2) != "QuizItemType" ||
                headerRow.Field<string>(3) != "Body" ||
                headerRow.Field<string>(4) != "QuizItemMaxScore" ||
                headerRow.Field<string>(5) != "MediaType" ||
                headerRow.Field<string>(6) != "MediaTitle" ||
                headerRow.Field<string>(7) != "MediaSource" ||
                headerRow.Field<string>(8) != "MediaUrl" ||
                headerRow.Field<string>(9) != "MediaIsSolution" ||
                headerRow.Field<string>(10) != "InteractionType" ||
                headerRow.Field<string>(11) != "Text" ||
                headerRow.Field<string>(12) != "InteractionMaxScore" ||
                headerRow.Field<string>(13) != "Choices" ||
                headerRow.Field<string>(14) != "Solutions" ||
                headerRow.Field<string>(15) != "LevenshteinTolerance" ||
                headerRow.Field<string>(16) != "FlagIfWithinTolerance")
            {
                errors.Add("The column headers are incorrect.");
                return true;
            }

            return false;
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
            _packagePath = Path.Combine(_quizrSettings.WebRootPath, _quizrSettings.ContentPath, _package.Hash);
            _logger.LogInformation($"Extracting to {_packagePath}");
            await Task.Run(() => ZipFile.ExtractToDirectory(_package.FullPath, _packagePath, true));
        }
    }
}