using System;
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
    [TestClass]
    public class RegistrationTests
    {
        private IUnitOfWork _unitOfWork;
        private Game _game;
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

            var quizRepo = _unitOfWork.GetCollection<Quiz>();
            var teamRepo = _unitOfWork.GetCollection<Team>();
            var gameRepo = _unitOfWork.GetCollection<Game>();

            _game = TestGame.GetGame();
            var quiz = TestQuiz.GetQuiz();
            var teams = TestTeams.GetTeams(teamRepo, _game.Id);
            _game.QuizId = quiz.Id;
            _game.TeamIds = teams.Select(t => t.Id).ToList();

            Task.WaitAll(
                quizRepo.AddAsync(quiz),
                teams.ToAsyncEnumerable().ForEachAsync(t => teamRepo.AddAsync(t)),
                gameRepo.AddAsync(_game));

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

        [TestMethod]
        public void TestGame_ChangeTeamMembersForInvalidTeam_ThrowsException()
        {
            // arrange
            var teamId = Guid.Empty;
            var notification = new ChangeTeamMembersNotification(_unitOfWork, _bus) {TeamMembers = "a,b,c", TeamId = teamId};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => notification.Execute()).Result;
            Assert.AreEqual("Invalid team id.", exception.Message);
            Assert.AreEqual(3, exception.ErrorCode);
            Assert.IsFalse(exception.IsBadRequest);
        }

        [TestMethod]
        public void TestGame_ChangeTeamMembersForValidTeam_TeamMembersChanged()
        {
            // arrange
            var teamId = _game.TeamIds[0]; // Team 1
            var notification = new ChangeTeamMembersNotification(_unitOfWork, _bus) {TeamMembers = "a,b,c", TeamId = teamId};

            // act
            notification.Execute().Wait();
            _unitOfWork.Commit();

            var team = _unitOfWork.GetCollection<Team>().GetAsync(teamId).Result;
            Assert.AreEqual("a,b,c", team.MemberNames);
        }
    }
}