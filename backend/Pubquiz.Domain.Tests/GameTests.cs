using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pubquiz.Logic.Requests;

namespace Pubquiz.Domain.Tests
{
    [TestClass]
    public class GameTests : InitializedTestBase
    {
        [TestMethod]
        public void TestGame_SubmitResponseForInvalidTeam_ThrowsException()
        {
            // arrange
            var notification = new SubmitInteractionResponseNotification(UnitOfWork, Bus)
            {
                TeamId = Guid.Empty,
                ChoiceOptionIds = new List<int> {3},
                InteractionId = 1,
                Response = string.Empty,
                QuestionId = Quiz.QuizSections.First().QuestionItems[0].Id
            };

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => notification.Execute()).Result;
            Assert.AreEqual(ErrorCodes.InvalidTeamId, exception.ErrorCode);
            Assert.AreEqual("Invalid team id.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestMethod]
        public void TestGame_SubmitResponseForInvalidQuestion_ThrowsException()
        {
            // arrange
            var notification = new SubmitInteractionResponseNotification(UnitOfWork, Bus)
            {
                TeamId = Game.TeamIds[0],
                ChoiceOptionIds = new List<int> {3},
                InteractionId = 1,
                Response = string.Empty,
                QuestionId = Guid.Empty
            };

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => notification.Execute()).Result;
            Assert.AreEqual(ErrorCodes.InvalidQuestionId, exception.ErrorCode);
            Assert.AreEqual("Invalid question id.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestMethod]
        public void TestGame_SubmitResponseForQuestionNotInCurrentQuiz_ThrowsException()
        {
            // arrange
            var notification = new SubmitInteractionResponseNotification(UnitOfWork, Bus)
            {
                TeamId = Game.TeamIds[0],
                ChoiceOptionIds = new List<int> {3},
                InteractionId = 1,
                Response = string.Empty,
                QuestionId = OtherQuestions[0].Id
            };

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => notification.Execute()).Result;
            Assert.AreEqual(ErrorCodes.QuestionNotInQuiz, exception.ErrorCode);
            Assert.AreEqual("This question doesn't belong to the quiz.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }


        [TestMethod]
        public void TestGame_SubmitResponseForQuestionNotInCurrentQuizSection_ThrowsException()
        {
            // arrange
            var notification = new SubmitInteractionResponseNotification(UnitOfWork, Bus)
            {
                TeamId = Game.TeamIds[0],
                ChoiceOptionIds = new List<int> {3},
                InteractionId = 1,
                Response = string.Empty,
                QuestionId = Quiz.QuizSections[1].QuestionItems[0].Id
            };

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => notification.Execute()).Result;
            Assert.AreEqual(ErrorCodes.QuestionNotInCurrentQuizSection, exception.ErrorCode);
            Assert.AreEqual("This question doesn't belong to the current quiz section.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestMethod]
        public void TestGame_SubmitResponseForInvalidInteraction_ThrowsException()
        {
            // arrange
            var notification = new SubmitInteractionResponseNotification(UnitOfWork, Bus)
            {
                TeamId = Game.TeamIds[0],
                ChoiceOptionIds = new List<int> {3},
                InteractionId = -1,
                Response = string.Empty,
                QuestionId = Quiz.QuizSections.First().QuestionItems[0].Id
            };

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => notification.Execute()).Result;
            Assert.AreEqual(ErrorCodes.InvalidInteractionId, exception.ErrorCode);
            Assert.AreEqual("Invalid interaction id.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);

         
        }
    }
}