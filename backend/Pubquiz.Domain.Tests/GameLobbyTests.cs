using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Requests;
using Pubquiz.Logic.Requests.Notifications;
using Pubquiz.Logic.Requests.Queries;
using Pubquiz.Persistence.Helpers;

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
            var query = new TeamLobbyViewModelQuery {TeamId = firstTeam.Id};

            // act
            var model = Mediator.Send(query).Result;

            // assert
            CollectionAssert.DoesNotContain(model.OtherTeamsInGame, firstTeam.Id);
        }

        [TestMethod]
        public void TestGame_TeamLobbyWithInvalidTeamId_ThrowsException()
        {
            // arrange
            var query = new TeamLobbyViewModelQuery {TeamId = Guid.Empty.ToShortGuidString()};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => Mediator.Send(query)).Result;
            Assert.AreEqual(ResultCode.InvalidEntityId, exception.ResultCode);
            Assert.AreEqual("Invalid TeamId.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }
        
        [TestMethod]
        public void TestGame_TeamLobbyWithNonOpenGame_ThrowsException()
        {
            // arrange
            var game = UnitOfWork.GetCollection<Game>().GetAsync(Game.Id).Result;
            game.SetState(GameState.Closed);
            UnitOfWork.GetCollection<Game>().UpdateAsync(game);
            var firstTeam = Teams[0];
            var query = new TeamLobbyViewModelQuery {TeamId = firstTeam.Id};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => Mediator.Send(query)).Result;
            Assert.AreEqual(ResultCode.LobbyUnavailableBecauseOfGameState, exception.ResultCode);
            Assert.AreEqual("The lobby for this game is not open.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestMethod]
        public void TestGame_QuizMasterSelectsValidGame_ReturnsUser()
        {
            // arrange
            var actorId = Users.First(u => u.UserName == "Quiz master 1").Id;
            var selectGameCommand = new SelectGameCommand {GameId = Game.Id, ActorId = actorId};

            // act
            Mediator.Send(selectGameCommand).Wait();

            // assert
            var updatedUser = UnitOfWork.GetCollection<User>().GetAsync(actorId).Result;
            Assert.IsNotNull(updatedUser);
            Assert.AreEqual(Game.Id, updatedUser.CurrentGameId);
        }

        [TestMethod]
        public void TestGame_QuizMasterSelectsInvalidGame_ThrowsException()
        {
            // arrange
            var actorId = Users.First(u => u.UserName == "Quiz master 1").Id;
            var command = new SelectGameCommand {GameId = Guid.Empty.ToShortGuidString(), ActorId = actorId};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => Mediator.Send(command)).Result;
            Assert.AreEqual(ResultCode.InvalidEntityId, exception.ResultCode);
            Assert.AreEqual("Invalid GameId.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestMethod]
        public void TestGame_InvalidQuizMasterSelectsGame_ThrowsException()
        {
            // arrange
            var actorId = Guid.Empty.ToShortGuidString();
            var command = new SelectGameCommand {GameId = Game.Id, ActorId = actorId};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => Mediator.Send(command)).Result;
            Assert.AreEqual(ResultCode.InvalidEntityId, exception.ResultCode);
            Assert.AreEqual("Invalid ActorId.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestMethod]
        public void TestGame_UnauthorizedQuizMasterSelectsGame_ThrowsException()
        {
            // arrange
            var actorId = Users.First(u => u.UserName == "Quiz master 2").Id;
            var command = new SelectGameCommand {GameId = Game.Id, ActorId = actorId};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => Mediator.Send(command)).Result;
            Assert.AreEqual(ResultCode.QuizMasterUnauthorizedForGame, exception.ResultCode);
            Assert.AreEqual($"Actor with id {actorId} is not authorized for game '{Game.Id}'", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }
    }
}