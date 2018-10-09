using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.ViewModels;
using Pubquiz.Logic.Requests;
using Pubquiz.Logic.Tools;

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
    }
}