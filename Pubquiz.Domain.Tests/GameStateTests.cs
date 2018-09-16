using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pubquiz.Domain.Tests
{
    [TestClass]
    public class GameStateTests
    {
        [TestMethod]
        public void GameInRunningState_Open_ThrowsException()
        {
            // arrange
            var game = new Game {State = GameState.Running};

            // act & assert
            var exception = Assert.ThrowsException<DomainException>(() => game.SetState(GameState.Open));
            Assert.AreEqual("Can only open the game from the closed state.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestMethod]
        public void GameInPausedState_Open_ThrowsException()
        {
            // arrange
            var game = new Game {State = GameState.Paused};

            // act & assert
            var exception = Assert.ThrowsException<DomainException>(() => game.SetState(GameState.Open));
            Assert.AreEqual("Can only open the game from the closed state.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestMethod]
        public void GameInFinishedState_Open_ThrowsException()
        {
            // arrange
            var game = new Game {State = GameState.Finished};

            // act & assert
            var exception = Assert.ThrowsException<DomainException>(() => game.SetState(GameState.Open));
            Assert.AreEqual("Can only open the game from the closed state.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestMethod]
        public void GameInOpenState_Close_StateChanged()
        {
            // arrange
            var game = new Game {State = GameState.Open};

            // act 
            game.SetState(GameState.Closed);

            // assert
            Assert.AreEqual(game.State, GameState.Closed);
        }

        [TestMethod]
        public void GameInOpenState_Run_StateChanged()
        {
            // arrange
            var game = new Game {State = GameState.Open};
            game.Teams.Add(new Team());

            // act 
            game.SetState(GameState.Running);

            // assert
            Assert.AreEqual(game.State, GameState.Running);
        }

        [TestMethod]
        public void GameInRunningState_Pause_StateChanged()
        {
            // arrange
            var game = new Game {State = GameState.Running};

            // act 
            game.SetState(GameState.Paused);

            // assert
            Assert.AreEqual(game.State, GameState.Paused);
        }

        [TestMethod]
        public void GameInRunningState_Finish_StateChanged()
        {
            // arrange
            var game = new Game {State = GameState.Running};

            // act 
            game.SetState(GameState.Finished);

            // assert
            Assert.AreEqual(game.State, GameState.Finished);
        }

        [TestMethod]
        public void GameInPausedState_Run_StateChanged()
        {
            // arrange
            var game = new Game {State = GameState.Paused};
            game.Teams.Add(new Team());

            // act 
            game.SetState(GameState.Running);

            // assert
            Assert.AreEqual(game.State, GameState.Running);
        }

        [TestMethod]
        public void GameInPausedState_Finish_StateChanged()
        {
            // arrange
            var game = new Game {State = GameState.Paused};
            game.Teams.Add(new Team());

            // act 
            game.SetState(GameState.Finished);

            // assert
            Assert.AreEqual(game.State, GameState.Finished);
        }

        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            var questionSet = new QuestionSet
            {
                Title = "Main quiz part",
                Category = "Anything goes"
            };

            var team = new Team
            {
                Name = "Testteam",
                MemberNames = new List<string> {"M88", "THT"}
            };

            var quiz = new Quiz {Title = "Testquiz"};
            quiz.QuestionSets.Add(questionSet);
            var game = new Game {Quiz = quiz};
            game.Teams.Add(team);


            // Act

            // Assert
            Assert.IsTrue(true);
        }
    }
}