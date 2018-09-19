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
        [TestMethod]
        public void TestGame_RegisterWithNewTeam_TeamRegistered()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var loggerFactory = new LoggerFactory();
            IRepositoryOptions inMemoryRepositoryOptions = new InMemoryDatabaseOptions();
            var repositoryFactory = new NoActionFactory(memoryCache, loggerFactory, inMemoryRepositoryOptions);

            var quizRepository = repositoryFactory.GetRepository<Quiz>();
            var teamRepository = repositoryFactory.GetRepository<Team>();
            var gameRepository = repositoryFactory.GetRepository<Game>();
            var game = TestGame.GetGame();
            var quiz = TestQuiz.GetQuiz();
            var teams = TestTeams.GetTeams(teamRepository, game.Id);
            game.QuizId = quiz.Id;
            game.TeamIds = teams.Select(t => t.Id).ToList();

            quizRepository.AddAsync(quiz).Wait();
            teams.ForEach(t => teamRepository.AddAsync(t).Wait());
            gameRepository.AddAsync(game).Wait();

            var command = new RegisterForGameCommand(repositoryFactory) {TeamName = "Team 4", Code = "JOINME"};
            var team = command.Execute().Result;

            Assert.AreEqual("Team 4", team.Name);
        }
    }
}