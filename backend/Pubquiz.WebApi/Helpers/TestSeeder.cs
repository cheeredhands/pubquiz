using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.Tools;
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
            var quizRepo = _unitOfWork.GetCollection<Quiz>();
            var teamRepo = _unitOfWork.GetCollection<Team>();
            var gameRepo = _unitOfWork.GetCollection<Game>();
            var userRepo = _unitOfWork.GetCollection<User>();
            var game = TestGame.GetGame();
            var quiz = TestQuiz.GetQuiz();
            var teams = TestTeams.GetTeams(teamRepo, game.Id);
            game.QuizId = quiz.Id;
            game.TeamIds = teams.Select(t => t.Id).ToList();
            var users = TestTeams.GetUsersFromTeams(teams).ToList();
            //quizRepo.AddAsync(quiz).Wait();
            //Task.WaitAll(teams.Select(t => teamRepo.AddAsync(t)).Cast<Task>().ToArray());
            Task.WaitAll(
                quizRepo.AddAsync(quiz),
                teams.ToAsyncEnumerable().ForEachAsync(t => teamRepo.AddAsync(t)),
                users.ToAsyncEnumerable().ForEachAsync(t => userRepo.AddAsync(t)), 
                gameRepo.AddAsync(game));

            //users.ForEach(u => userRepo.AddAsync(u).Wait());
            //gameRepo.AddAsync(game).Wait();
        }
    }
}