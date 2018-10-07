using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Handlers;
using Pubquiz.Logic.Messages;
using Pubquiz.Logic.Requests;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Pubquiz.Persistence.NoAction;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Persistence.InMem;
using Rebus.Routing.TypeBased;
using Rebus.Transport.InMem;

namespace Pubquiz.Domain.Tests
{
    [TestCategory("Account actions")]
    [TestClass]
    public class AccountTests
    {
        private IUnitOfWork _unitOfWork;
        private Game _game;
        private List<User> _users;
        private IBus _bus;
        private ILoggerFactory _loggerFactory;
        private InMemorySubscriberStore _inMemorySubscriberStore;

        [TestInitialize]
        public void Initialize()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            _loggerFactory = new LoggerFactory();
            _loggerFactory.AddConsole();
            ICollectionOptions inMemoryCollectionOptions = new InMemoryDatabaseOptions();
            _unitOfWork = new NoActionUnitOfWork(memoryCache, _loggerFactory, inMemoryCollectionOptions);

            var quizCollection = _unitOfWork.GetCollection<Quiz>();
            var userCollection = _unitOfWork.GetCollection<User>();
            var teamCollection = _unitOfWork.GetCollection<Team>();
            var gameCollection = _unitOfWork.GetCollection<Game>();
            _users = TestUsers.GetUsers();
            _game = TestGame.GetGame(_users.Where(u => u.UserName == "Quiz master 1").Select(u => u.Id));
            var quiz = TestQuiz.GetQuiz();
            var teams = TestTeams.GetTeams(teamCollection, _game.Id);
            _game.QuizId = quiz.Id;
            _game.TeamIds = teams.Select(t => t.Id).ToList();

            Task.WaitAll(
                quizCollection.AddAsync(quiz),
                teams.ToAsyncEnumerable().ForEachAsync(t => teamCollection.AddAsync(t)),
                _users.ToAsyncEnumerable().ForEachAsync(u => userCollection.AddAsync(u)),
                gameCollection.AddAsync(_game));

            // set up bus
            var activator = new BuiltinHandlerActivator();
            activator.Register((bus, messageContext) => new ScoringHandler(_unitOfWork, bus));
            activator.Register(() => new ClientNotificationHandler(_loggerFactory));

            // needed so the inmemory subscription store will be centralized
            _inMemorySubscriberStore = new InMemorySubscriberStore();

            Configure.With(activator).Logging(l => l.ColoredConsole())
                .Transport(t => t.UseInMemoryTransport(new InMemNetwork(true), "Messages"))
                .Routing(r => r.TypeBased().MapAssemblyOf<TeamMembersChanged>("Messages"))
                .Subscriptions(s => s.StoreInMemory(_inMemorySubscriberStore))
                .Start();

            _bus = activator.Bus;
            _bus.SubscribeByScanningForHandlers(Assembly.Load("Pubquiz.Logic"));
        }

        [TestCategory("Team registration")]
        [TestMethod]
        public void TestGame_RegisterWithCorrectNewTeam_TeamRegistered()
        {
            // arrange
            var command = new RegisterForGameCommand(_unitOfWork, _bus) {TeamName = "Team 4", Code = "JOINME"};

            // act
            var team = command.Execute().Result;
            _unitOfWork.Commit();

            // assert
            Assert.AreEqual("Team 4", team.Name);
        }

        [TestCategory("Team registration")]
        [TestMethod]
        public void TestGame_UseTeamRecoveryCode_RecoveryCodeAccepted()
        {
            // arrange 
            var firstTeamId = _game.TeamIds[0];
            var firstTeam = _unitOfWork.GetCollection<Team>().GetAsync(firstTeamId).Result;
            var command = new RegisterForGameCommand(_unitOfWork, _bus) {TeamName = "", Code = firstTeam.RecoveryCode};

            // act
            var team = command.Execute().Result;
            _unitOfWork.Commit();

            // assert
            team.RecoveryCode = firstTeam.RecoveryCode;
        }

        [TestCategory("Team registration")]
        [TestMethod]
        public void TestGame_RegisterWithInvalidCode_ThrowsException()
        {
            // arrange
            var command = new RegisterForGameCommand(_unitOfWork, _bus) {TeamName = "Team 4", Code = "INVALIDCODE"};

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
            var command = new RegisterForGameCommand(_unitOfWork, _bus) {TeamName = "Team 3", Code = "JOINME"};

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
            var teamId = _game.TeamIds[0]; // Team 1
            var command = new ChangeTeamNameCommand(_unitOfWork, _bus) {NewName = "Team 1a", TeamId = teamId};

            // act
            var team = command.Execute().Result;
            _unitOfWork.Commit();

            // assert
            Assert.AreEqual("Team 1a", team.Name);
            Assert.AreEqual("Team%201a", team.UserName);
        }

        [TestCategory("Team registration")]
        [TestMethod]
        public void TestGame_ChangeTeamNameForInvalidTeam_ThrowsException()
        {
            // arrange
            var teamId = Guid.Empty;
            var command = new ChangeTeamNameCommand(_unitOfWork, _bus) {NewName = "Team 1a", TeamId = teamId};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => command.Execute()).Result;
            Assert.AreEqual("Invalid team id.", exception.Message);
            Assert.AreEqual(3, exception.ErrorCode);
            Assert.IsFalse(exception.IsBadRequest);
        }

        [TestCategory("Team registration")]
        [TestMethod]
        public void TestGame_ChangeTeamNameToTakenName_ThrowsException()
        {
            // arrange
            var teamId = _game.TeamIds[0]; // Team 1
            var command = new ChangeTeamNameCommand(_unitOfWork, _bus) {NewName = "Team 2", TeamId = teamId};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => command.Execute()).Result;
            Assert.AreEqual("Team name is taken.", exception.Message);
            Assert.AreEqual(2, exception.ErrorCode);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestCategory("Team registration")]
        [TestMethod]
        public void TestGame_ChangeTeamMembersForInvalidTeam_ThrowsException()
        {
            // arrange
            var teamId = Guid.Empty;
            var notification = new ChangeTeamMembersNotification(_unitOfWork, _bus)
                {TeamMembers = "a,b,c", TeamId = teamId};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => notification.Execute()).Result;
            Assert.AreEqual("Invalid team id.", exception.Message);
            Assert.AreEqual(3, exception.ErrorCode);
            Assert.IsFalse(exception.IsBadRequest);
        }

        [TestCategory("Team registration")]
        [TestMethod]
        public void TestGame_ChangeTeamMembersForValidTeam_TeamMembersChanged()
        {
            // arrange
            var teamId = _game.TeamIds[0]; // Team 1
            var notification = new ChangeTeamMembersNotification(_unitOfWork, _bus)
                {TeamMembers = "a,b,c", TeamId = teamId};

            // act
            notification.Execute().Wait();
            _unitOfWork.Commit();

            var team = _unitOfWork.GetCollection<Team>().GetAsync(teamId).Result;
            Assert.AreEqual("a,b,c", team.MemberNames);
        }

        [TestCategory("Team registration")]
        [TestMethod]
        public void TestGame_DeleteTeamWithInvalidTeamId_ThrowsException()
        {
            // arrange
            var teamId = Guid.Empty;
            var user = _users.First(u => u.UserName == "Quiz master 1");
            var notification = new DeleteTeamNotification(_unitOfWork, _bus) {ActorId = user.Id, TeamId = teamId};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => notification.Execute()).Result;
            Assert.AreEqual(ErrorCodes.InvalidTeamId, exception.ErrorCode);
            Assert.AreEqual("Invalid team id.", exception.Message);
            Assert.IsFalse(exception.IsBadRequest);
        }

        [TestCategory("Team registration")]
        [TestMethod]
        public void TestGame_DeleteTeamWithInvalidActorId_ThrowsException()
        {
            // arrange
            var teamId = _game.TeamIds[0]; // Team 1
            var actorId = Guid.Empty;
            var notification = new DeleteTeamNotification(_unitOfWork, _bus) {ActorId = actorId, TeamId = teamId};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => notification.Execute()).Result;
            Assert.AreEqual(ErrorCodes.InvalidUserId, exception.ErrorCode);
            Assert.AreEqual("Invalid actor id.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestCategory("Team registration")]
        [TestMethod]
        public void TestGame_DeleteTeamByUnauthorizedQuizMaster_ThrowsException()
        {
            // arrange
            var teamId = _game.TeamIds[0]; // Team 1
            var user = _users.First(u => u.UserName == "Quiz master 2");
            var notification = new DeleteTeamNotification(_unitOfWork, _bus) {ActorId = user.Id, TeamId = teamId};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => notification.Execute()).Result;
            Assert.AreEqual(ErrorCodes.QuizMasterUnauthorizedForGame, exception.ErrorCode);
            Assert.AreEqual($"Actor with id {user.Id} is not authorized for game '{_game.Id}'", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestCategory("Team registration")]
        [TestMethod]
        public void TestGame_DeleteValidTeamWithValidActorId_TeamDeleted()
        {
            // arrange
            var teamId = _game.TeamIds[0]; // Team 1
            var user = _users.First(u => u.UserName == "Quiz master 1");
            var notification = new DeleteTeamNotification(_unitOfWork, _bus) {ActorId = user.Id, TeamId = teamId};

            // act
            notification.Execute().Wait();
            _unitOfWork.Commit();

            // assert
            Assert.IsNull(_unitOfWork.GetCollection<Team>().GetAsync(teamId).Result);
            CollectionAssert.DoesNotContain(_unitOfWork.GetCollection<Game>().GetAsync(_game.Id).Result.TeamIds,
                teamId);
        }

        [TestCategory("User login")]
        [TestMethod]
        public void TestGame_LoginAdmin_AdminLoggedIn()
        {
            // arrange
            var command = new LoginCommand(_unitOfWork, _bus) {UserName = "Admin", Password = "secret123"};

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
            var command = new LoginCommand(_unitOfWork, _bus) {UserName = "Quiz master 1", Password = "qm1"};

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
            var command = new LoginCommand(_unitOfWork, _bus) {UserName = "Admin", Password = "wrong password"};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => command.Execute()).Result;
            Assert.AreEqual(ErrorCodes.InvalidUserNameOrPassword, exception.ErrorCode);
            Assert.AreEqual("Invalid username or password.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestCategory("User login")]
        [TestMethod]
        public void TestGame_LoginInvalidUserName_ThrowsException()
        {
            // arrange
            var command = new LoginCommand(_unitOfWork, _bus) {UserName = "NonexistentUser", Password = "secret123"};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => command.Execute()).Result;
            Assert.AreEqual(ErrorCodes.InvalidUserNameOrPassword, exception.ErrorCode);
            Assert.AreEqual("Invalid username or password.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }
    }
}