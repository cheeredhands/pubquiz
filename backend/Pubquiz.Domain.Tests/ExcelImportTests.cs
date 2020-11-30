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
        
            var quizrPackage = await command.Execute();
        
            // assert
            var quizrPackageCollection = UnitOfWork.GetCollection<QuizrPackage>();
            var quizrPackageRetrieved = await quizrPackageCollection.GetAsync(quizrPackage.Id);
            Assert.AreEqual(1, quizrPackage.QuizIds.Count);
            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            var quiz = await quizCollection.GetAsync(quizrPackage.QuizIds.First());
            Assert.AreEqual("PéCé-pubquiz 2019", quiz.Title);
        }
        [TestMethod]
        public async Task Oki2020QuizExcelQuizPackage_Import_CorrectQuizNameImported()
        {
            // arrange
            var quizrSettings = new QuizrSettings
            {
                BaseUrl = "https://localhost:5001",
                ContentPath = "quiz"
            };
            await using var stream = File.OpenRead("testfiles/OKI-Kerstquiz-2020.zip");
            var command =
                new ImportZippedExcelQuizCommand(UnitOfWork, Bus, stream, "OKI-Kerstquiz-2020.zip", quizrSettings, LoggerFactory);
            // act
        
            var quizrPackage = await command.Execute();
        
            // assert
            var quizrPackageCollection = UnitOfWork.GetCollection<QuizrPackage>();
            var quizrPackageRetrieved = await quizrPackageCollection.GetAsync(quizrPackage.Id);
            Assert.AreEqual(1, quizrPackage.QuizIds.Count);
            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            var quiz = await quizCollection.GetAsync(quizrPackage.QuizIds.First());
            Assert.AreEqual("OKI-Kerstquiz 2020", quiz.Title);
        }
    }
}