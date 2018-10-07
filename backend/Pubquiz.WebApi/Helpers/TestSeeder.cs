using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.WebApi.Helpers
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

        public void SeedTestSet()
        {
            _logger.LogInformation("Seeding test set.");
            var quizCollection = _unitOfWork.GetCollection<Quiz>();
            var teamCollection = _unitOfWork.GetCollection<Team>();
            var userCollection = _unitOfWork.GetCollection<User>();
            var gameCollection = _unitOfWork.GetCollection<Game>();
            var users = TestUsers.GetUsers();
            var quiz = TestQuiz.GetQuiz();
            var game = TestGame.GetGame(users.Where(u => u.UserRole == UserRole.QuizMaster).Select(u => u.Id), 0,
                quiz.QuizSections[0].Id);
            var teams = TestTeams.GetTeams(teamCollection, game.Id);
            game.QuizId = quiz.Id;
            game.TeamIds = teams.Select(t => t.Id).ToList();

            Task.WaitAll(
                quizCollection.AddAsync(quiz),
                teams.ToAsyncEnumerable().ForEachAsync(t => teamCollection.AddAsync(t)),
                users.ToAsyncEnumerable().ForEachAsync(u => userCollection.AddAsync(u)),
                gameCollection.AddAsync(game));
        }
    }
}