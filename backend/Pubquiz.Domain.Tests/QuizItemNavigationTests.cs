using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Requests;
using Pubquiz.Logic.Requests.Commands;

namespace Pubquiz.Domain.Tests
{
    [TestClass]
    public class QuizItemNavigationTests : InitializedTestBase
    {
        [TestMethod]
        public void TestQuizAtFirstSectionAndFirstQuestion_NavigateOneBackward_CorrectSectionAndQuizItem()
        {
            // Arrange
            var command = new NavigateToItemByOffsetCommand(UnitOfWork, Mediator)
            {
                Offset = -1,
                GameId = Game.Id,
                ActorId = Game.QuizMasterIds.First()
            };

            // Act
            var unused = command.Execute().Result;

            // Assert
            var game = UnitOfWork.GetCollection<Game>().GetAsync(Game.Id).Result;
            Assert.AreEqual(1, game.CurrentSectionIndex);
            Assert.AreEqual(1, game.CurrentQuizItemIndexInSection);
            Assert.AreEqual(1, game.CurrentQuizItemIndexInTotal);
            Assert.AreEqual(0, game.CurrentQuestionIndexInTotal);
            Assert.AreEqual(Quiz.QuizSections[0].Id, game.CurrentSectionId);
        }

        [TestMethod]
        public void TestQuizAtFirstSectionAndFirstQuestion_NavigateOneForward_CorrectSectionAndQuizItem()
        {
            // Arrange
            var command = new NavigateToItemByOffsetCommand(UnitOfWork, Mediator)
            {
                Offset = 1,
                GameId = Game.Id,
                ActorId = Game.QuizMasterIds.First()
            };

            // Act
            var unused = command.Execute().Result;

            // Assert
            var game = UnitOfWork.GetCollection<Game>().GetAsync(Game.Id).Result;
            Assert.AreEqual(2, game.CurrentSectionIndex);
            Assert.AreEqual(2, game.CurrentQuizItemIndexInSection);
            Assert.AreEqual(3, game.CurrentQuizItemIndexInTotal);
            Assert.AreEqual(2, game.CurrentQuestionIndexInTotal);
            Assert.AreEqual(Quiz.QuizSections[1].Id, game.CurrentSectionId);
        }

        [TestMethod]
        public void TestQuizAtFirstSectionAndFirstQuestion_NavigateFourForward_CorrectSectionAndQuizItem()
        {
            // Arrange
            var command = new NavigateToItemByOffsetCommand(UnitOfWork, Mediator)
            {
                Offset = 4,
                GameId = Game.Id,
                ActorId = Game.QuizMasterIds.First()
            };

            // Act
            var unused = command.Execute().Result;

            // Assert
            var game = UnitOfWork.GetCollection<Game>().GetAsync(Game.Id).Result;
            Assert.AreEqual(3, game.CurrentSectionIndex);
            Assert.AreEqual(2, game.CurrentQuizItemIndexInSection);
            Assert.AreEqual(6, game.CurrentQuizItemIndexInTotal);
            Assert.AreEqual(5, game.CurrentQuestionIndexInTotal);
            Assert.AreEqual(Quiz.QuizSections[2].Id, game.CurrentSectionId);
        }

        [TestMethod]
        public void TestQuizAtFirstSectionAndFirstQuestion_NavigateTenForward_LocationAtEnd()
        {
            // Arrange
            var command = new NavigateToItemByOffsetCommand(UnitOfWork, Mediator)
            {
                Offset = 10,
                GameId = Game.Id,
                ActorId = Game.QuizMasterIds.First()
            };

            // Act
            var unused = command.Execute().Result;

            // Assert
            var game = UnitOfWork.GetCollection<Game>().GetAsync(Game.Id).Result;
            Assert.AreEqual(3, game.CurrentSectionIndex);
            Assert.AreEqual(3, game.CurrentQuizItemIndexInSection);
            Assert.AreEqual(7, game.CurrentQuizItemIndexInTotal);
            Assert.AreEqual(6, game.CurrentQuestionIndexInTotal);
            Assert.AreEqual(Quiz.QuizSections[2].Id, game.CurrentSectionId);
        }

        [TestMethod]
        public void TestQuizAtFirstSectionAndFirstQuestion_NavigateFourBackward_LocationAtStart()
        {
            // Arrange
            var command = new NavigateToItemByOffsetCommand(UnitOfWork, Mediator)
            {
                Offset = -4,
                GameId = Game.Id,
                ActorId = Game.QuizMasterIds.First()
            };

            // Act
            var unused = command.Execute().Result;

            // Assert
            var game = UnitOfWork.GetCollection<Game>().GetAsync(Game.Id).Result;
            Assert.AreEqual(1, game.CurrentSectionIndex);
            Assert.AreEqual(1, game.CurrentQuizItemIndexInSection);
            Assert.AreEqual(1, game.CurrentQuizItemIndexInTotal);
            Assert.AreEqual(0, game.CurrentQuestionIndexInTotal);
            Assert.AreEqual(Quiz.QuizSections[0].Id, game.CurrentSectionId);
        }
    }
}