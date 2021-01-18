using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Requests;
using Pubquiz.Logic.Requests.Commands;
using Pubquiz.Logic.Requests.Notifications;
using Pubquiz.Persistence.Extensions;

namespace Pubquiz.Domain.Tests
{
    [TestCategory("Account actions")]
    [TestClass]
    public class AccountTests : InitializedTestBase
    {
        [TestCategory("Team registration")]
        [TestMethod]
        public void TestGame_RegisterWithCorrectNewTeam_TeamRegistered()
        {
            // arrange
            var command = new RegisterForGameCommand(UnitOfWork, Mediator) {Name = "Team 4", Code = "JOINME"};

            // act
            var team = command.Execute().Result;

            // assert
            Assert.AreEqual("Team 4", team.Name);
        }

        [TestCategory("Team registration")]
        [TestMethod]
        public void TestGame_RegisterWithCorrectNewTeam_TeamInGame()
        {
            // arrange
            var command = new RegisterForGameCommand(UnitOfWork, Mediator) {Name = "Team 4", Code = "JOINME"};

            // act
            var team = command.Execute().Result;

            // assert
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = gameCollection.GetAsync(team.CurrentGameId).Result;
            CollectionAssert.Contains(game.TeamIds, team.Id);
            Assert.AreEqual("Team 4", team.Name);
        }

        [TestCategory("Team registration")]
        [TestMethod]
        public void TestGame_UseTeamRecoveryCode_RecoveryCodeAccepted()
        {
            // arrange 
            var firstTeamId = Game.TeamIds[0];
            var firstTeam = UnitOfWork.GetCollection<Team>().GetAsync(firstTeamId).Result;
            var command = new RegisterForGameCommand(UnitOfWork, Mediator) {Name = "", Code = firstTeam.RecoveryCode};

            // act
            var team = command.Execute().Result;

            // assert
            team.RecoveryCode = firstTeam.RecoveryCode;
        }

        [TestCategory("Team registration")]
        [TestMethod]
        public void TestGame_RegisterWithInvalidCode_ThrowsException()
        {
            // arrange
            var command = new RegisterForGameCommand(UnitOfWork, Mediator) {Name = "Team 4", Code = "INVALIDCODE"};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => command.Execute()).Result;
            Assert.AreEqual("Invalid code.", exception.Message);
            Assert.IsFalse(exception.IsBadRequest);
        }

        [TestCategory("Team registration")]
        [TestMethod]
        public void TestGame_RegisterWithExistingTeamName_ThrowsException()
        {
            // arrange
            var command = new RegisterForGameCommand(UnitOfWork, Mediator) {Name = "Team 3", Code = "JOINME"};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => command.Execute()).Result;
            Assert.AreEqual("Team name is taken.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestCategory("Team registration")]
        [TestMethod]
        public void TestGame_ChangeTeamNameForValidTeam_TeamNameChanged()
        {
            // arrange
            var teamId = Game.TeamIds[0]; // Team 1
            var notification = new ChangeTeamNameNotification {NewName = "Team 1a", TeamId = teamId};

            // act
            Mediator.Publish(notification).Wait();
            Thread.Sleep(2000);

            // assert
            var team = UnitOfWork.GetCollection<Team>().GetAsync(teamId).Result;
            Assert.AreEqual("Team 1a", team.Name);
            Assert.AreEqual("Team 1a", team.UserName);
        }

        [TestCategory("Team registration")]
        [TestMethod]
        public void TestGame_ChangeTeamNameForInvalidTeam_ThrowsException()
        {
            // arrange
            var teamId = Guid.Empty.ToShortGuidString();
            var notification = new ChangeTeamNameNotification {NewName = "Team 1a", TeamId = teamId};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => Mediator.Publish(notification)).Result;
            Assert.AreEqual("Invalid TeamId.", exception.Message);
            Assert.AreEqual(ResultCode.InvalidTeamId, exception.ResultCode);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestCategory("Team registration")]
        [TestMethod]
        public void TestGame_ChangeTeamNameToTakenName_ThrowsException()
        {
            // arrange
            var teamId = Game.TeamIds[0]; // Team 1
            var notification = new ChangeTeamNameNotification {NewName = "Team 2", TeamId = teamId};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => Mediator.Publish(notification)).Result;
            Assert.AreEqual("Team name is taken.", exception.Message);
            Assert.AreEqual(ResultCode.TeamNameIsTaken, exception.ResultCode);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestCategory("Team registration")]
        [TestMethod]
        public void TestGame_ChangeTeamMembersForInvalidTeam_ThrowsException()
        {
            // arrange
            var teamId = Guid.Empty.ToShortGuidString();
            var notification = new ChangeTeamMembersNotification {TeamMembers = "a,b,c", TeamId = teamId};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => Mediator.Publish(notification)).Result;
            Assert.AreEqual("Invalid TeamId.", exception.Message);
            Assert.AreEqual(ResultCode.InvalidTeamId, exception.ResultCode);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestCategory("Team registration")]
        [TestMethod]
        public void TestGame_ChangeTeamMembersForValidTeam_TeamMembersChanged()
        {
            // arrange
            var teamId = Game.TeamIds[0]; // Team 1
            var notification = new ChangeTeamMembersNotification {TeamMembers = "a,b,c", TeamId = teamId};

            // act
            Mediator.Publish(notification).Wait();

            var team = UnitOfWork.GetCollection<Team>().GetAsync(teamId).Result;
            Assert.AreEqual("a,b,c", team.MemberNames);
        }

        [TestCategory("Team registration")]
        [TestMethod]
        public void TestGame_DeleteTeamWithInvalidTeamId_ThrowsException()
        {
            // arrange
            var teamId = Guid.Empty.ToShortGuidString();
            var user = Users.First(u => u.UserName == "Quiz master 1");
            var notification = new DeleteTeamNotification {ActorId = user.Id, TeamId = teamId};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => Mediator.Publish(notification)).Result;
            Assert.AreEqual(ResultCode.InvalidTeamId, exception.ResultCode);
            Assert.AreEqual("Invalid TeamId.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestCategory("Team registration")]
        [TestMethod]
        public void TestGame_DeleteTeamWithInvalidActorId_ThrowsException()
        {
            // arrange
            var teamId = Game.TeamIds[0]; // Team 1
            var actorId = Guid.Empty.ToShortGuidString();
            var notification = new DeleteTeamNotification {ActorId = actorId, TeamId = teamId};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => Mediator.Publish(notification)).Result;
            Assert.AreEqual(ResultCode.InvalidUserId, exception.ResultCode);
            Assert.AreEqual("Invalid ActorId.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestCategory("Team registration")]
        [TestMethod]
        public void TestGame_DeleteTeamByUnauthorizedQuizMaster_ThrowsException()
        {
            // arrange
            var teamId = Game.TeamIds[0]; // Team 1
            var user = Users.First(u => u.UserName == "Quiz master 2");
            var notification = new DeleteTeamNotification {ActorId = user.Id, TeamId = teamId};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => Mediator.Publish(notification)).Result;
            Assert.AreEqual(ResultCode.QuizMasterUnauthorizedForGame, exception.ResultCode);
            Assert.AreEqual($"Actor with id {user.Id} is not authorized for game '{Game.Id}'", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestCategory("Team registration")]
        [TestMethod]
        public void TestGame_DeleteValidTeamWithValidActorId_TeamDeleted()
        {
            // arrange
            var teamId = Game.TeamIds[0]; // Team 1
            var user = Users.First(u => u.UserName == "Quiz master 1");
            var notification = new DeleteTeamNotification {ActorId = user.Id, TeamId = teamId};

            // act
            Mediator.Publish(notification).Wait();

            // assert
            Assert.IsNull(UnitOfWork.GetCollection<Team>().GetAsync(teamId).Result);
            CollectionAssert.DoesNotContain(UnitOfWork.GetCollection<Game>().GetAsync(Game.Id).Result.TeamIds,
                teamId);
        }

        [TestCategory("User login")]
        [TestMethod]
        public void TestGame_LoginAdmin_AdminLoggedIn()
        {
            // arrange
            var command = new LoginCommand(UnitOfWork, Mediator) {UserName = "Admin", Password = "secret123"};

            // act
            var user = command.Execute().Result;

            // assert
            Assert.AreEqual(UserRole.Admin, user.UserRole);
        }

        [TestCategory("User login")]
        [TestMethod]
        public void TestGame_LoginQuizMaster_QuizMasterLoggedIn()
        {
            // arrange
            var command = new LoginCommand(UnitOfWork, Mediator) {UserName = "Quiz master 1", Password = "qm1"};

            // act
            var user = command.Execute().Result;

            // assert
            Assert.AreEqual(UserRole.QuizMaster, user.UserRole);
        }

        [TestCategory("User login")]
        [TestMethod]
        public void TestGame_LoginInvalidPassword_ThrowsException()
        {
            // arrange
            var command = new LoginCommand(UnitOfWork, Mediator) {UserName = "Admin", Password = "wrong password"};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => command.Execute()).Result;
            Assert.AreEqual(ResultCode.InvalidUserNameOrPassword, exception.ResultCode);
            Assert.AreEqual("Invalid username or password.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestCategory("User login")]
        [TestMethod]
        public void TestGame_LoginInvalidUserName_ThrowsException()
        {
            // arrange
            var command = new LoginCommand(UnitOfWork, Mediator) {UserName = "NonexistentUser", Password = "secret123"};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => command.Execute()).Result;
            Assert.AreEqual(ResultCode.InvalidUserNameOrPassword, exception.ResultCode);
            Assert.AreEqual("Invalid username or password.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }
    }
}