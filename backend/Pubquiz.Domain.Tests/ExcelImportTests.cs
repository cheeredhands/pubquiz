using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Requests.Commands;
// ReSharper disable StringLiteralTypo

namespace Pubquiz.Domain.Tests
{
    [TestClass]
    public class ExcelImportTests : InitializedTestBase
    {
        [TestMethod, Ignore]
        public async Task PeCeExcelQuizPackage_Import_CorrectQuizNameImported()
        {
            // arrange
            await using var stream = File.OpenRead("testfiles/PeCe.zip");
            var command = new ImportZippedExcelQuizCommand
            {
                ActorId = Users.First(u => u.UserRole == UserRole.QuizMaster).Id,
                FileStream = stream,
                FileName = "PeCe.zip"
            };
            // act

            var quizrPackage = await Mediator.Send(command);

            // assert
            var quizrPackageCollection = UnitOfWork.GetCollection<QuizrPackage>();
            var quizrPackageRetrieved = await quizrPackageCollection.GetAsync(quizrPackage.Id);
            Assert.AreEqual(1, quizrPackageRetrieved.QuizViewModels.Count);
            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            var quizRef = await quizCollection.GetAsync(quizrPackageRetrieved.QuizViewModels.First().Id);
            Assert.AreEqual("PéCé-pubquiz 2019", quizRef.Title);
        }

        [TestMethod]
        public async Task Fryslan2020QuizExcelQuizPackage_Import_CorrectQuizNameImported()
        {
            // arrange
            await using var stream = File.OpenRead("testfiles/Fryslan-Kerstquiz-2020.zip");
            var command = new ImportZippedExcelQuizCommand
            {
                ActorId = Users.First(u => u.UserRole == UserRole.QuizMaster).Id,
                FileStream = stream,
                FileName = "Fryslan-Kerstquiz-2020.zip"
            };
            // act

            var quizrPackage = await Mediator.Send(command);

            // assert
            var quizrPackageCollection = UnitOfWork.GetCollection<QuizrPackage>();
            var quizrPackageRetrieved = await quizrPackageCollection.GetAsync(quizrPackage.Id);
            Assert.AreEqual(1, quizrPackageRetrieved.QuizViewModels.Count);
            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            var quizRef = await quizCollection.GetAsync(quizrPackageRetrieved.QuizViewModels.First().Id);
            Assert.AreEqual("TH en Anne's krystkwis 2020", quizRef.Title);
        }

        [TestMethod]
        public async Task Oki2020QuizExcelQuizPackage_Import_CorrectQuizNameImported()
        {
            // arrange
            await using var stream = File.OpenRead("testfiles/OKI-Kerstquiz-2020.zip");
            var command = new ImportZippedExcelQuizCommand
            {
                ActorId = Users.First(u => u.UserRole == UserRole.QuizMaster).Id,
                FileStream = stream,
                FileName = "OKI-Kerstquiz-2020.zip"
            };
            // act

            var quizrPackage = await Mediator.Send(command);

            // assert
            var quizrPackageCollection = UnitOfWork.GetCollection<QuizrPackage>();
            var quizrPackageRetrieved = await quizrPackageCollection.GetAsync(quizrPackage.Id);
            Assert.AreEqual(1, quizrPackageRetrieved.QuizViewModels.Count);
            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            var quizRef = await quizCollection.GetAsync(quizrPackageRetrieved.QuizViewModels.First().Id);
            Assert.AreEqual("OKI-Kerstquiz 2020", quizRef.Title);
        }
    }
}