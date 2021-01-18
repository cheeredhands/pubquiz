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
                WebRootPath = "",
                ContentPath = "quiz"
            };
            await using var stream = File.OpenRead("testfiles/PeCe.zip");
            var command =
                new ImportZippedExcelQuizCommand(UnitOfWork, Mediator, stream, "PeCe.zip", quizrSettings, LoggerFactory);
            command.ActorId = Users.First(u => u.UserRole == UserRole.QuizMaster).Id;
            // act

            var quizrPackage = await command.Execute();

            // assert
            var quizrPackageCollection = UnitOfWork.GetCollection<QuizrPackage>();
            var quizrPackageRetrieved = await quizrPackageCollection.GetAsync(quizrPackage.Id);
            Assert.AreEqual(1, quizrPackage.QuizRefs.Count);
            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            var quizRef = await quizCollection.GetAsync(quizrPackage.QuizRefs.First().Id);
            Assert.AreEqual("PéCé-pubquiz 2019", quizRef.Title);
        }

        [TestMethod]
        public async Task Fryslan2020QuizExcelQuizPackage_Import_CorrectQuizNameImported()
        {
            // arrange
            var quizrSettings = new QuizrSettings
            {
                BaseUrl = "https://localhost:5001",
                WebRootPath = "",
                ContentPath = "quiz"
            };
            await using var stream = File.OpenRead("testfiles/Fryslan-Kerstquiz-2020.zip");
            var command =
                new ImportZippedExcelQuizCommand(UnitOfWork, Mediator, stream, "Fryslan-Kerstquiz-2020.zip", quizrSettings,
                    LoggerFactory);
            command.ActorId = Users.First(u => u.UserRole == UserRole.QuizMaster).Id;
            // act

            var quizrPackage = await command.Execute();

            // assert
            var quizrPackageCollection = UnitOfWork.GetCollection<QuizrPackage>();
            var quizrPackageRetrieved = await quizrPackageCollection.GetAsync(quizrPackage.Id);
            Assert.AreEqual(1, quizrPackage.QuizRefs.Count);
            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            var quizRef = await quizCollection.GetAsync(quizrPackage.QuizRefs.First().Id);
            Assert.AreEqual("TH en Anne's krystkwis 2020", quizRef.Title);
        }

        [TestMethod]
        public async Task Oki2020QuizExcelQuizPackage_Import_CorrectQuizNameImported()
        {
            // arrange
            var quizrSettings = new QuizrSettings
            {
                BaseUrl = "https://localhost:5001",
                WebRootPath = "",
                ContentPath = "quiz"
            };
            await using var stream = File.OpenRead("testfiles/OKI-Kerstquiz-2020.zip");
            var command =
                new ImportZippedExcelQuizCommand(UnitOfWork, Mediator, stream, "OKI-Kerstquiz-2020.zip", quizrSettings,
                    LoggerFactory);
            command.ActorId = Users.First(u => u.UserRole == UserRole.QuizMaster).Id;
            // act

            var quizrPackage = await command.Execute();

            // assert
            var quizrPackageCollection = UnitOfWork.GetCollection<QuizrPackage>();
            var quizrPackageRetrieved = await quizrPackageCollection.GetAsync(quizrPackage.Id);
            Assert.AreEqual(1, quizrPackage.QuizRefs.Count);
            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            var quizRef = await quizCollection.GetAsync(quizrPackage.QuizRefs.First().Id);
            Assert.AreEqual("OKI-Kerstquiz 2020", quizRef.Title);
        }
    }
}