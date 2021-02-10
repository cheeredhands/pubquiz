using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pubquiz.Logic.Requests.Notifications;
using Pubquiz.Persistence.Extensions;

namespace Pubquiz.Domain.Tests
{
    [TestClass]
    public class GameSubmitResponseTests : InitializedTestBase
    {
        [TestMethod]
        public void TestGame_SubmitResponseForInvalidTeam_ThrowsException()
        {
            // arrange
            var command = new SubmitInteractionResponseCommand
            {
                TeamId = Guid.Empty.ToShortGuidString(),
                ChoiceOptionIds = new List<int> {3},
                InteractionId = 1,
                Response = string.Empty,
                QuizItemId = Quiz.QuizSections[1].QuestionItemRefs[0].Id
            };

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => Mediator.Send(command)).Result;
            Assert.AreEqual(ResultCode.InvalidEntityId, exception.ResultCode);
            Assert.AreEqual("Invalid TeamId.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestMethod]
        public void TestGame_SubmitResponseForInvalidQuestion_ThrowsException()
        {
            // arrange
            var command = new SubmitInteractionResponseCommand
            {
                TeamId = Game.TeamIds[0],
                ChoiceOptionIds = new List<int> {3},
                InteractionId = 1,
                Response = string.Empty,
                QuizItemId = Guid.Empty.ToShortGuidString()
            };

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => Mediator.Send(command)).Result;
            Assert.AreEqual(ResultCode.InvalidEntityId, exception.ResultCode);
            Assert.AreEqual("Invalid QuizItemId.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestMethod]
        public void TestGame_SubmitResponseForQuestionNotInCurrentQuiz_ThrowsException()
        {
            // arrange
            var command = new SubmitInteractionResponseCommand
            {
                TeamId = Game.TeamIds[0],
                ChoiceOptionIds = new List<int> {3},
                InteractionId = 1,
                Response = string.Empty,
                QuizItemId = OtherQuestions[0].Id
            };

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => Mediator.Send(command)).Result;
            Assert.AreEqual(ResultCode.QuestionNotInQuiz, exception.ResultCode);
            Assert.AreEqual("This question doesn't belong to the quiz.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }


        [TestMethod]
        public void TestGame_SubmitResponseForQuestionNotInCurrentQuizSection_ThrowsException()
        {
            // arrange
            var command = new SubmitInteractionResponseCommand
            {
                TeamId = Game.TeamIds[0],
                ChoiceOptionIds = new List<int> {3},
                InteractionId = 1,
                Response = string.Empty,
                QuizItemId = Quiz.QuizSections[2].QuestionItemRefs[0].Id
            };

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => Mediator.Send(command)).Result;
            Assert.AreEqual(ResultCode.QuestionNotInCurrentQuizSection, exception.ResultCode);
            Assert.AreEqual("This question doesn't belong to the current quiz section.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestMethod]
        public void TestGame_SubmitResponseForInvalidInteraction_ThrowsException()
        {
            // arrange
            var command = new SubmitInteractionResponseCommand
            {
                TeamId = Game.TeamIds[0],
                ChoiceOptionIds = new List<int> {3},
                InteractionId = -1,
                Response = string.Empty,
                QuizItemId = Quiz.QuizSections[1].QuestionItemRefs[0].Id
            };

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => Mediator.Send(command)).Result;
            Assert.AreEqual(ResultCode.InvalidEntityId, exception.ResultCode);
            Assert.AreEqual("Invalid InteractionId.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }
    }
}