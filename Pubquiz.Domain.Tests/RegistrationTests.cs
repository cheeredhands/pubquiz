using System;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.Requests;
using Pubquiz.Domain.Tools;
using Pubquiz.Repository;
using Pubquiz.Repository.NoAction;

namespace Pubquiz.Domain.Tests
{
    [TestClass]
    public class RegistrationTests
    {
        private IRepositoryFactory _repositoryFactory;

        [TestInitialize]
        public void Initialize()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var loggerFactory = new LoggerFactory();
            IRepositoryOptions inMemoryRepositoryOptions = new InMemoryDatabaseOptions();
            _repositoryFactory = new NoActionFactory(memoryCache, loggerFactory, inMemoryRepositoryOptions);

            var quizRepository = _repositoryFactory.GetRepository<Quiz>();
            var teamRepository = _repositoryFactory.GetRepository<Team>();
            var gameRepository = _repositoryFactory.GetRepository<Game>();
            var game = TestGame.GetGame();
            var quiz = TestQuiz.GetQuiz();
            var teams = TestTeams.GetTeams(teamRepository, game.Id);
            game.QuizId = quiz.Id;
            game.TeamIds = teams.Select(t => t.Id).ToList();

            quizRepository.AddAsync(quiz).Wait();
            teams.ForEach(t => teamRepository.AddAsync(t).Wait());
            gameRepository.AddAsync(game).Wait();
        }

        [TestMethod]
        public void TestGame_RegisterWithCorrectNewTeam_TeamRegistered()
        {
            // arrange
            var command = new RegisterForGameCommand(_repositoryFactory) {TeamName = "Team 4", Code = "JOINME"};

            // act
            var team = command.Execute().Result;

            // assert
            Assert.AreEqual("Team 4", team.Name);
        }

        [TestMethod]
        public void TestGame_RegisterWithInvalidCode_ThrowsException()
        {
            // arrange
            var command = new RegisterForGameCommand(_repositoryFactory) {TeamName = "Team 4", Code = "INVALIDCODE"};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => command.Execute()).Result;
            Assert.AreEqual("Invalid code.", exception.Message);
            Assert.IsFalse(exception.IsBadRequest);
        }
        
        [TestMethod]
        public void TestGame_RegisterWithExistingTeamName_ThrowsException()
        {
            // arrange
            var command = new RegisterForGameCommand(_repositoryFactory) {TeamName = "Team 3", Code = "JOINME"};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => command.Execute()).Result;
            Assert.AreEqual("Team name is taken.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }
    }
}