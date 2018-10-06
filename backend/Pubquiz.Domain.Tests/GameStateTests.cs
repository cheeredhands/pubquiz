using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;

namespace Pubquiz.Domain.Tests
{
    [TestClass]
    public class GameStateTests
    {
        [TestMethod]
        public void GameNotInClosedState_Open_ThrowsException()
        {
            foreach (GameState state in Enum.GetValues(typeof(GameState)))
            {
                if (state == GameState.Closed || state == GameState.Open)
                {
                    continue;
                }

                // arrange
                var game = new Game {State = state};

                // act & assert
                var exception = Assert.ThrowsException<DomainException>(() => game.SetState(GameState.Open));
                Assert.AreEqual("Can only open the game from the closed state.", exception.Message);
                Assert.IsTrue(exception.IsBadRequest);
            }
        }

        [TestMethod]
        public void GameNotInOpenState_Close_ThrowsException()
        {
            foreach (GameState state in Enum.GetValues(typeof(GameState)))
            {
                if (state == GameState.Open || state == GameState.Closed)
                {
                    continue;
                }

                // arrange
                var game = new Game {State = state};

                // act & assert
                var exception = Assert.ThrowsException<DomainException>(() => game.SetState(GameState.Closed));
                Assert.AreEqual("Can only close the game from the open state.", exception.Message);
                Assert.IsTrue(exception.IsBadRequest);
            }
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
        public void GameInClosedStateWithoutAQuiz_Open_ThrowsException()
        {
            // arrange
            var game = new Game {State = GameState.Closed, Title = "Testquiz"};

            // act & assert
            var exception = Assert.ThrowsException<DomainException>(() => game.SetState(GameState.Open));
            Assert.AreEqual("Can't open the game without a quiz and/or a title.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestMethod]
        public void GameInClosedStateWithoutATitle_Open_ThrowsException()
        {
            // arrange
            var quiz = TestQuiz.GetQuiz();
            var game = new Game {State = GameState.Closed, QuizId = quiz.Id};

            // act & assert
            var exception = Assert.ThrowsException<DomainException>(() => game.SetState(GameState.Open));
            Assert.AreEqual("Can't open the game without a quiz and/or a title.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestMethod]
        public void GameInOpenState_Run_StateChanged()
        {
            // arrange
            var game = new Game {State = GameState.Open};
            game.TeamIds.Add(Guid.NewGuid());

            // act 
            game.SetState(GameState.Running);

            // assert
            Assert.AreEqual(game.State, GameState.Running);
        }


        [TestMethod]
        public void GameInOpenStateWithoutTeams_Run_ThrowsException()
        {
            // arrange
            var game = new Game {State = GameState.Open};


            // act & assert
            var exception = Assert.ThrowsException<DomainException>(() => game.SetState(GameState.Running));
            Assert.AreEqual("Can't start the game without teams.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestMethod]
        public void GameNotInOpenOrPausedState_Run_ThrowsException()
        {
            foreach (GameState state in Enum.GetValues(typeof(GameState)))
            {
                if (state == GameState.Open || state == GameState.Paused || state == GameState.Running)
                {
                    continue;
                }

                // arrange
                var game = new Game {State = state};

                // act & assert
                var exception = Assert.ThrowsException<DomainException>(() => game.SetState(GameState.Running));
                Assert.AreEqual("Can only start the game from the open and paused states.", exception.Message);
                Assert.IsTrue(exception.IsBadRequest);
            }
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
        public void GameNotInRunningState_Pause_ThrowsException()
        {
            foreach (GameState state in Enum.GetValues(typeof(GameState)))
            {
                if (state == GameState.Running || state == GameState.Paused)
                {
                    continue;
                }

                // arrange
                var game = new Game {State = state};

                // act & assert
                var exception = Assert.ThrowsException<DomainException>(() => game.SetState(GameState.Paused));
                Assert.AreEqual("Can only pause the game from the running state.", exception.Message);
                Assert.IsTrue(exception.IsBadRequest);
            }
        }

        [TestMethod]
        public void GameInPausedState_Run_StateChanged()
        {
            // arrange
            var game = new Game {State = GameState.Paused};
            game.TeamIds.Add(Guid.NewGuid());

            // act 
            game.SetState(GameState.Running);

            // assert
            Assert.AreEqual(game.State, GameState.Running);
        }

        [TestMethod]
        public void GameNotInRunningOrPausedState_Finish_ThrowsException()
        {
            foreach (GameState state in Enum.GetValues(typeof(GameState)))
            {
                if (state == GameState.Running || state == GameState.Paused || state == GameState.Finished)
                {
                    continue;
                }

                // arrange
                var game = new Game {State = state};

                // act & assert
                var exception = Assert.ThrowsException<DomainException>(() => game.SetState(GameState.Finished));
                Assert.AreEqual("Can only finish the game from the running and paused states.", exception.Message);
                Assert.IsTrue(exception.IsBadRequest);
            }
        }

        [TestMethod]
        public void GameInPausedState_Finish_StateChanged()
        {
            // arrange
            var game = new Game {State = GameState.Paused};
            game.TeamIds.Add(Guid.NewGuid());

            // act 
            game.SetState(GameState.Finished);

            // assert
            Assert.AreEqual(game.State, GameState.Finished);
        }
    }
}