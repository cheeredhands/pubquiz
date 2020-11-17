using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Requests.Commands;
using Pubquiz.Logic.Tools;

namespace Pubquiz.Domain.Tests
{
    [TestClass]
    public class ExcelImportTests : InitializedTestBase
    {
        [TestMethod]
        public async Task PeCeExcelQuizPackage_Import_CorrectQuizNameImported()
        {
            // arrange
            var quizrSettings = new QuizrSettings
            {
                BaseUrl = "https://localhost:5001",
                ContentPath = "quiz"
            };
            await using var stream = File.OpenRead("testfiles/PeCe.zip");
            var command =
                new ImportZippedExcelQuizCommand(UnitOfWork, Bus, stream, "PeCe.zip", quizrSettings, LoggerFactory);
            // act
        
            var quizrPackageId = await command.Execute();
        
            // assert
            var quizrPackageCollection = UnitOfWork.GetCollection<QuizrPackage>();
            var quizrPackage = await quizrPackageCollection.GetAsync(quizrPackageId);
            Assert.AreEqual(1, quizrPackage.QuizIds.Count);
            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            var quiz = await quizCollection.GetAsync(quizrPackage.QuizIds.First());
            Assert.AreEqual("PéCé-pubquiz 2019", quiz.Title);
        }
    }
}