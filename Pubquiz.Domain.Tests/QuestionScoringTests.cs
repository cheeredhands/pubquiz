using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pubquiz.Domain.Models;

namespace Pubquiz.Domain.Tests
{
    [TestClass]
    public class QuestionScoringTests
    {
        [TestMethod]
        public void TestQuiz_AnswerMcIncorrect_ZeroTotalScore()
        {
            // Arrange
            var quiz = TestQuiz.GetQuiz();
            var mcQuestion = quiz.QuestionSets.First().Questions[0];

            var answer = new Answer(quiz.QuestionSets.First().Id, mcQuestion.Id);
            answer.InteractionResponses.Add(new InteractionResponse(1, new[] {1}));

            // Act
            answer.Score(mcQuestion);

            // Assert
            Assert.AreEqual(0, answer.TotalScore);
        }

        [TestMethod]
        public void TestQuiz_AnswerMcCorrect_OneTotalScore()
        {
            // Arrange
            var quiz = TestQuiz.GetQuiz();
            var mcQuestion = quiz.QuestionSets.First().Questions.First();

            var answer = new Answer(quiz.QuestionSets.First().Id, mcQuestion.Id);
            answer.InteractionResponses.Add(new InteractionResponse(1, new[] {3}));

            // Act
            answer.Score(mcQuestion);

            // Assert
            Assert.AreEqual(1, answer.TotalScore);
        }

        [TestMethod]
        public void TestQuiz_AnswerMcIncorrectWithTwoChoiceOptions_ZeroTotalScore()
        {
            // Arrange
            var quiz = TestQuiz.GetQuiz();
            var mcQuestion = quiz.QuestionSets.First().Questions.First();

            var answer = new Answer(quiz.QuestionSets.First().Id, mcQuestion.Id);
            answer.InteractionResponses.Add(new InteractionResponse(1, new[] {1, 3}));

            // Act
            answer.Score(mcQuestion);

            // Assert
            Assert.AreEqual(0, answer.TotalScore);
        }

        [TestMethod]
        public void TestQuiz_AnswerMrCorrect_OneTotalScore()
        {
            // Arrange
            var quiz = TestQuiz.GetQuiz();
            var mrQuestion = quiz.QuestionSets.First().Questions[1];

            var answer = new Answer(quiz.QuestionSets.First().Id, mrQuestion.Id);
            answer.InteractionResponses.Add(new InteractionResponse(1, new[] {4, 3}));

            // Act
            answer.Score(mrQuestion);

            // Assert
            Assert.AreEqual(1, answer.TotalScore);
        }

        [TestMethod]
        public void TestQuiz_AnswerMrSemiIncorrect_ZeroTotalScore()
        {
            // Arrange
            var quiz = TestQuiz.GetQuiz();
            var mrQuestion = quiz.QuestionSets.First().Questions[1];

            var answer = new Answer(quiz.QuestionSets.First().Id, mrQuestion.Id);
            answer.InteractionResponses.Add(new InteractionResponse(1, new[] {1, 3}));

            // Act
            answer.Score(mrQuestion);

            // Assert
            Assert.AreEqual(0, answer.TotalScore);
        }

        [TestMethod]
        public void TestQuiz_AnswerMrIncorrect_ZeroTotalScore()
        {
            // Arrange
            var quiz = TestQuiz.GetQuiz();
            var mrQuestion = quiz.QuestionSets.First().Questions[1];

            var answer = new Answer(quiz.QuestionSets.First().Id, mrQuestion.Id);
            answer.InteractionResponses.Add(new InteractionResponse(1, new[] {1, 2}));

            // Act
            answer.Score(mrQuestion);

            // Assert
            Assert.AreEqual(0, answer.TotalScore);
        }

        [TestMethod]
        public void TestQuiz_AnswerMrIncorrectTooManyChoiceOptionIds_ZeroTotalScore()
        {
            // Arrange
            var quiz = TestQuiz.GetQuiz();
            var mrQuestion = quiz.QuestionSets.First().Questions[1];

            var answer = new Answer(quiz.QuestionSets.First().Id, mrQuestion.Id);
            answer.InteractionResponses.Add(new InteractionResponse(1, new[] {1, 2, 3, 4}));

            // Act
            answer.Score(mrQuestion);

            // Assert
            Assert.AreEqual(0, answer.TotalScore);
        }

        [TestMethod]
        public void TestQuiz_AnswerMrIncorrectNoChoiceOptionIds_ZeroTotalScore()
        {
            // Arrange
            var quiz = TestQuiz.GetQuiz();
            var mrQuestion = quiz.QuestionSets.First().Questions[1];

            var answer = new Answer(quiz.QuestionSets.First().Id, mrQuestion.Id);
            answer.InteractionResponses.Add(new InteractionResponse(1, new int[] { }));

            // Act
            answer.Score(mrQuestion);

            // Assert
            Assert.AreEqual(0, answer.TotalScore);
        }

        [TestMethod]
        public void TestQuiz_AnswerSaCorrect_OneTotalScore()
        {
            // Arrange
            var quiz = TestQuiz.GetQuiz();
            var saQuestion = quiz.QuestionSets.First().Questions[2];

            var answer = new Answer(quiz.QuestionSets.First().Id, saQuestion.Id);
            answer.InteractionResponses.Add(new InteractionResponse(1, "answer"));

            // Act
            answer.Score(saQuestion);

            // Assert
            Assert.AreEqual(1, answer.TotalScore);
        }

        [TestMethod]
        public void TestQuiz_AnswerSaIncorrectWrongAnswer_ZeroTotalScore()
        {
            // Arrange
            var quiz = TestQuiz.GetQuiz();
            var saQuestion = quiz.QuestionSets.First().Questions[2];

            var answer = new Answer(quiz.QuestionSets.First().Id, saQuestion.Id);
            answer.InteractionResponses.Add(new InteractionResponse(1, "wrong answer"));

            // Act
            answer.Score(saQuestion);

            // Assert
            Assert.AreEqual(0, answer.TotalScore);
            Assert.IsTrue(answer.FlaggedForManualCorrection);
        }

        [TestMethod]
        public void TestQuiz_AnswerSaIncorrectNoAnswer_ZeroTotalScore()
        {
            // Arrange
            var quiz = TestQuiz.GetQuiz();
            var saQuestion = quiz.QuestionSets.First().Questions[2];

            var answer = new Answer(quiz.QuestionSets.First().Id, saQuestion.Id);
            answer.InteractionResponses.Add(new InteractionResponse(1, string.Empty));

            // Act
            answer.Score(saQuestion);

            // Assert
            Assert.AreEqual(0, answer.TotalScore);
            Assert.IsTrue(answer.FlaggedForManualCorrection);
        }

        [TestMethod]
        public void TestQuiz_AnswerSaMultipleSolutionCorrect1_OneTotalScore()
        {
            // Arrange
            var quiz = TestQuiz.GetQuiz();
            var saQuestion = quiz.QuestionSets.First().Questions[3];

            var answer = new Answer(quiz.QuestionSets.First().Id, saQuestion.Id);
            answer.InteractionResponses.Add(new InteractionResponse(1, "answer"));

            // Act
            answer.Score(saQuestion);

            // Assert
            Assert.AreEqual(1, answer.TotalScore);
        }

        [TestMethod]
        public void TestQuiz_AnswerSaMultipleSolutionCorrect2_OneTotalScore()
        {
            // Arrange
            var quiz = TestQuiz.GetQuiz();
            var saQuestion = quiz.QuestionSets.First().Questions[3];

            var answer = new Answer(quiz.QuestionSets.First().Id, saQuestion.Id);
            answer.InteractionResponses.Add(new InteractionResponse(1, "ansver"));

            // Act
            answer.Score(saQuestion);

            // Assert
            Assert.AreEqual(1, answer.TotalScore);
        }

        [TestMethod]
        public void TestQuiz_AnswerMultipleSaCorrect_ThreeTotalScore()
        {
            // Arrange
            var quiz = TestQuiz.GetQuiz();
            var multipleSaQuestion = quiz.QuestionSets.First().Questions[4];

            var answer = new Answer(quiz.QuestionSets.First().Id, multipleSaQuestion.Id);
            answer.InteractionResponses.Add(new InteractionResponse(1, "answer1"));
            answer.InteractionResponses.Add(new InteractionResponse(2, "answer2"));

            // Act
            answer.Score(multipleSaQuestion);

            // Assert
            Assert.AreEqual(3, answer.TotalScore);
        }

        [TestMethod]
        public void TestQuiz_AnswerMultipleSaFirstCorrect_OneTotalScore()
        {
            // Arrange
            var quiz = TestQuiz.GetQuiz();
            var multipleSaQuestion = quiz.QuestionSets.First().Questions[4];

            var answer = new Answer(quiz.QuestionSets.First().Id, multipleSaQuestion.Id);
            answer.InteractionResponses.Add(new InteractionResponse(1, "answer1"));
            answer.InteractionResponses.Add(new InteractionResponse(2, "wrong answer"));

            // Act
            answer.Score(multipleSaQuestion);

            // Assert
            Assert.AreEqual(1, answer.TotalScore);
        }

        [TestMethod]
        public void TestQuiz_AnswerMultipleSaSecondCorrect_TwoTotalScore()
        {
            // Arrange
            var quiz = TestQuiz.GetQuiz();
            var multipleSaQuestion = quiz.QuestionSets.First().Questions[4];

            var answer = new Answer(quiz.QuestionSets.First().Id, multipleSaQuestion.Id);
            answer.InteractionResponses.Add(new InteractionResponse(1, "wrong answer"));
            answer.InteractionResponses.Add(new InteractionResponse(2, "answer2"));

            // Act
            answer.Score(multipleSaQuestion);

            // Assert
            Assert.AreEqual(2, answer.TotalScore);
        }

        [TestMethod]
        public void TestQuiz_AnswerEt_FlaggedForManualCorrectionNoScore()
        {
            // Arrange
            var quiz = TestQuiz.GetQuiz();
            var etQuestion = quiz.QuestionSets.First().Questions[5];

            var answer = new Answer(quiz.QuestionSets.First().Id, etQuestion.Id);
            answer.InteractionResponses.Add(new InteractionResponse(1, "an answer"));

            // Act
            answer.Score(etQuestion);

            // Assert
            Assert.AreEqual(0, answer.TotalScore);
            Assert.IsTrue(answer.FlaggedForManualCorrection);
        }
        
        [TestMethod]
        public void TestQuizAnswerEtFlaggedForManualCorrectionNoScore_ManualScoreCorrect_OneTotalScore()
        {
            // Arrange
            var quiz = TestQuiz.GetQuiz();
            var etQuestion = quiz.QuestionSets.First().Questions[5];

            var answer = new Answer(quiz.QuestionSets.First().Id, etQuestion.Id);
            answer.InteractionResponses.Add(new InteractionResponse(1, "an answer"));

            // Act
            //answer.Score(etQuestion);
            etQuestion.Score(answer);
            answer.InteractionResponses[0].Correct(true);
            //answer.Score(etQuestion);
            etQuestion.Score(answer);
            
            // Assert
            Assert.AreEqual(1, answer.TotalScore);
            Assert.IsFalse(answer.FlaggedForManualCorrection);
            Assert.IsTrue(answer.InteractionResponses[0].ManuallyCorrected);
        }
    }
}