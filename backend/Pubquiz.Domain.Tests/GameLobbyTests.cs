using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Requests;

namespace Pubquiz.Domain.Tests
{
    [TestClass]
    public class GameLobbyTests : InitializedTestBase
    {
        [TestMethod]
        public void TestGame_TeamLobby_ShouldntShowOwnTeam()
        {
            // arrange
            var firstTeam = Teams[0];
            var query = new TeamLobbyViewModelQuery(UnitOfWork) {TeamId = firstTeam.Id};

            // act
            var model = query.Execute().Result;

            // assert
            CollectionAssert.DoesNotContain(model.OtherTeamsInGame, firstTeam.Id);
        }

        [TestMethod]
        public void TestGame_TeamLobbyWithInvalidTeamId_ThrowsException()
        {
            // arrange
            var query = new TeamLobbyViewModelQuery(UnitOfWork) {TeamId = Guid.Empty};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => query.Execute()).Result;
            Assert.AreEqual(ErrorCodes.InvalidTeamId, exception.ErrorCode);
            Assert.AreEqual("Invalid team id, or you're not a team.", exception.Message);
            Assert.IsFalse(exception.IsBadRequest);
        }


        [TestMethod]
        public void TestGame_TeamLobbyWithNonOpenGame_ThrowsException()
        {
            // arrange
            var game = UnitOfWork.GetCollection<Game>().GetAsync(Game.Id).Result;
            game.SetState(GameState.Closed);
            UnitOfWork.GetCollection<Game>().UpdateAsync(game);
            UnitOfWork.Commit();
            var firstTeam = Teams[0];
            var query = new TeamLobbyViewModelQuery(UnitOfWork) {TeamId = firstTeam.Id};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => query.Execute()).Result;
            Assert.AreEqual(ErrorCodes.LobbyUnavailableBecauseOfGameState, exception.ErrorCode);
            Assert.AreEqual("The lobby for this game is not open.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestMethod]
        public void TestGame_QuizMasterSelectsValidGame_ReturnsUser()
        {
            // arrange
            var actorId = Users.First(u => u.UserName == "Quiz master 1").Id;
            var command = new SelectGameCommand(UnitOfWork, Bus) {GameId = Game.Id, ActorId = actorId};

            // act
            var user = command.Execute().Result;

            // assert
            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void TestGame_QuizMasterSelectsInvalidGame_ThrowsException()
        {
            // arrange
            var actorId = Users.First(u => u.UserName == "Quiz master 1").Id;
            var command = new SelectGameCommand(UnitOfWork, Bus) {GameId = Guid.Empty, ActorId = actorId};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => command.Execute()).Result;
            Assert.AreEqual(ErrorCodes.InvalidGameId, exception.ErrorCode);
            Assert.AreEqual("Invalid game id.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestMethod]
        public void TestGame_InvalidQuizMasterSelectsGame_ThrowsException()
        {
            // arrange
            var actorId = Guid.Empty;
            var command = new SelectGameCommand(UnitOfWork, Bus) {GameId = Game.Id, ActorId = actorId};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => command.Execute()).Result;
            Assert.AreEqual(ErrorCodes.InvalidUserId, exception.ErrorCode);
            Assert.AreEqual("Invalid user id.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestMethod]
        public void TestGame_UnauthorizedQuizMasterSelectsGame_ThrowsException()
        {
            // arrange
            var actorId = Users.First(u => u.UserName == "Quiz master 2").Id;
            var command = new SelectGameCommand(UnitOfWork, Bus) {GameId = Game.Id, ActorId = actorId};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => command.Execute()).Result;
            Assert.AreEqual(ErrorCodes.QuizMasterUnauthorizedForGame, exception.ErrorCode);
            Assert.AreEqual($"Actor with id {actorId} is not authorized for game '{Game.Id}'", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }
    }
}