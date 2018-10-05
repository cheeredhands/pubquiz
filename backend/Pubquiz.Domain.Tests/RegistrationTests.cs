using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.Requests;
using Pubquiz.Domain.Tools;
using Pubquiz.Persistence;
using Pubquiz.Persistence.NoAction;

namespace Pubquiz.Domain.Tests
{
    [TestClass]
    public class RegistrationTests
    {
        private IUnitOfWork _unitOfWork;

        private Game _game;

        [TestInitialize]
        public void Initialize()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var loggerFactory = new LoggerFactory();
            ICollectionOptions inMemoryCollectionOptions = new InMemoryDatabaseOptions();
            _unitOfWork = new NoActionUnitOfWork(memoryCache, loggerFactory, inMemoryCollectionOptions);

            var quizRepo = _unitOfWork.GetCollection<Quiz>();
            var teamRepo = _unitOfWork.GetCollection<Team>();
            var gameRepo = _unitOfWork.GetCollection<Game>();

            _game = TestGame.GetGame();
            var quiz = TestQuiz.GetQuiz();
            var teams = TestTeams.GetTeams(teamRepo, _game.Id);
            _game.QuizId = quiz.Id;
            _game.TeamIds = teams.Select(t => t.Id).ToList();

            // quizRepo.AddAsync(quiz).Wait();
            //teams.ForEach(t => teamRepo.AddAsync(t).Wait());
            // gameRepo.AddAsync(_game).Wait();

            Task.WaitAll(
                quizRepo.AddAsync(quiz),
                teams.ToAsyncEnumerable().ForEachAsync(t => teamRepo.AddAsync(t)),
                // users.ToAsyncEnumerable().ForEachAsync(t => userRepo.AddAsync(t)),
                gameRepo.AddAsync(_game));
        }

        [TestMethod]
        public void TestGame_RegisterWithCorrectNewTeam_TeamRegistered()
        {
            // arrange
            var command = new RegisterForGameCommand(_unitOfWork) {TeamName = "Team 4", Code = "JOINME"};

            // act
            var team = command.Execute().Result;

            // assert
            Assert.AreEqual("Team 4", team.Name);
        }

        [TestMethod]
        public void TestGame_UseTeamRecoveryCode_RecoveryCodeAccepted()
        {
            // arrange 
            var firstTeamId = _game.TeamIds[0];
            var firstTeam = _unitOfWork.GetCollection<Team>().GetAsync(firstTeamId).Result;
            var command = new RegisterForGameCommand(_unitOfWork) {TeamName = "", Code = firstTeam.RecoveryCode};

            // act
            var team = command.Execute().Result;

            // assert
            team.RecoveryCode = firstTeam.RecoveryCode;
        }

        [TestMethod]
        public void TestGame_RegisterWithInvalidCode_ThrowsException()
        {
            // arrange
            var command = new RegisterForGameCommand(_unitOfWork) {TeamName = "Team 4", Code = "INVALIDCODE"};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => command.Execute()).Result;
            Assert.AreEqual("Invalid code.", exception.Message);
            Assert.IsFalse(exception.IsBadRequest);
        }

        [TestMethod]
        public void TestGame_RegisterWithExistingTeamName_ThrowsException()
        {
            // arrange
            var command = new RegisterForGameCommand(_unitOfWork) {TeamName = "Team 3", Code = "JOINME"};

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
            var command = new ChangeTeamNameCommand(_unitOfWork) {NewName = "Team 1a", TeamId = teamId};

            // act
            var team = command.Execute().Result;

            // assert
            Assert.AreEqual("Team 1a", team.Name);
            Assert.AreEqual("Team%201a", team.UserName);
        }

        [TestMethod]
        public void TestGame_ChangeTeamNameForInvalidTeam_ThrowsException()
        {
            // arrange
            var teamId = Guid.Empty;
            var command = new ChangeTeamNameCommand(_unitOfWork) {NewName = "Team 1a", TeamId = teamId};

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
            var command = new ChangeTeamNameCommand(_unitOfWork) {NewName = "Team 2", TeamId = teamId};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => command.Execute()).Result;
            Assert.AreEqual("Team name is taken.", exception.Message);
            Assert.AreEqual(2, exception.ErrorCode);
            Assert.IsTrue(exception.IsBadRequest);
        }
    }
}