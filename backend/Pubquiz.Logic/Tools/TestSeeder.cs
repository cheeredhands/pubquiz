using System.Linq;
using Microsoft.Extensions.Logging;
using Pubquiz.Domain.Models;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Tools
{
    public class TestSeeder
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public TestSeeder(IUnitOfWork unitOfWork, ILoggerFactory loggerFactory)
        {
            _unitOfWork = unitOfWork;
            _logger = loggerFactory.CreateLogger<TestSeeder>();
        }

        public void SeedSeedSet(string mediaBaseUrl)
        {
            _logger.LogInformation("Seeding the seed set.");
            var quizCollection = _unitOfWork.GetCollection<Quiz>();
            var teamCollection = _unitOfWork.GetCollection<Team>();
            var userCollection = _unitOfWork.GetCollection<User>();
            var gameCollection = _unitOfWork.GetCollection<Game>();
            var quizItemCollection = _unitOfWork.GetCollection<QuizItem>();
            var users = SeedUsers.GetUsers();
            var quizFactory = new PeCePubquiz2019(mediaBaseUrl);
            var quiz = quizFactory.GetQuiz();
            var quizItems = quizFactory.QuizItems;
            var game = SeedGame.GetGame(users.Where(u => u.UserName == "Quiz master 1").Select(u => u.Id), quiz);
            users.First(u => u.UserName == "Quiz master 1").GameIds.Add(game.Id);
            users.First(u => u.UserName == "Quiz master 1").CurrentGameId = game.Id;
            var teams = SeedTeams.GetTeams(teamCollection, game.Id);
            var teamUsers = SeedTeams.GetUsersFromTeams(teams);
            foreach (var team in teams)
            {
                _logger.LogInformation($"{team.Name}: {team.RecoveryCode}");
            }

            game.QuizId = quiz.Id;
            game.TeamIds = teams.Select(t => t.Id).ToList();

            quizCollection.AddAsync(quiz).Wait();
            foreach (var quizItem in quizItems)
            {
                quizItemCollection.AddAsync(quizItem).Wait();
            }
            foreach (var team in teams)
            {
                teamCollection.AddAsync(team).Wait();
            }

            foreach (var user in users)
            {
                userCollection.AddAsync(user).Wait();
            }

            foreach (var teamUser in teamUsers)
            {
                userCollection.AddAsync(teamUser).Wait();
            }

            gameCollection.AddAsync(game).Wait();
        }

        public void SeedTestSet()
        {
            _logger.LogInformation("Seeding test set.");
            var quizCollection = _unitOfWork.GetCollection<Quiz>();
            var teamCollection = _unitOfWork.GetCollection<Team>();
            var userCollection = _unitOfWork.GetCollection<User>();
            var gameCollection = _unitOfWork.GetCollection<Game>();
            var quizItemCollection = _unitOfWork.GetCollection<QuizItem>();
            var users = TestUsers.GetUsers();
            var quiz = TestQuiz.GetQuiz();
            var quizItems = TestQuiz.GetQuizItems();
            var game = TestGame.GetGame(users.Where(u => u.UserName == "Quiz master 1").Select(u => u.Id), quiz);
            users.First(u => u.UserName == "Quiz master 1").GameIds.Add(game.Id);
            users.First(u => u.UserName == "Quiz master 1").CurrentGameId = game.Id;
            var teams = TestTeams.GetTeams(teamCollection, game.Id);
            foreach (var team in teams)
            {
                _logger.LogInformation($"{team.Name}: {team.RecoveryCode}");
            }

            game.QuizId = quiz.Id;
            game.TeamIds = teams.Select(t => t.Id).ToList();

            quizCollection.AddAsync(quiz).Wait();
            foreach (var quizItem in quizItems)
            {
                quizItemCollection.AddAsync(quizItem).Wait();
            }
            foreach (var team in teams)
            {
                teamCollection.AddAsync(team).Wait();
            }

            foreach (var user in users)
            {
                userCollection.AddAsync(user).Wait();
            }

            gameCollection.AddAsync(game).Wait();
        }
    }
}