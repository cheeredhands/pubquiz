using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Requests.Commands;
using Pubquiz.Logic.Requests.Notifications;
using Pubquiz.Persistence;
using Rebus.Bus;

namespace Pubquiz.Logic.Tools
{
    public class TestSeeder
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IBus _bus;
        private readonly QuizrSettings _quizrSettings;
        private readonly ILoggerFactory _loggerFactory;

        public TestSeeder(IUnitOfWork unitOfWork, ILoggerFactory loggerFactory, IBus bus, QuizrSettings quizrSettings)
        {
            _unitOfWork = unitOfWork;
            _bus = bus;
            _quizrSettings = quizrSettings;
            _loggerFactory = loggerFactory;
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
            var gameRef = new GameRef
                {Id = game.Id, Title = game.Title, QuizTitle = game.QuizTitle, InviteCode = game.InviteCode};
            users.First(u => u.UserName == "Quiz master 1").GameRefs.Add(gameRef);
            users.First(u => u.UserName == "Quiz master 1").QuizRefs.Add(new QuizRef
            {
                Id = quiz.Id,
                Title = quiz.Title,
                GameRefs = new List<GameRef> {gameRef}
            });
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

        public async Task SeedZippedExcelQuiz(string filePath, string fileName, string inviteCode,
            string gameTitle)
        {
            var path = Path.Combine(_quizrSettings.WebRootPath, filePath);
            await using var stream = File.OpenRead(path);
            var command =
                new ImportZippedExcelQuizCommand(_unitOfWork, _bus, stream, fileName, _quizrSettings, _loggerFactory);
            var userCollection = _unitOfWork.GetCollection<User>();
            var qmId = userCollection.FirstOrDefaultAsync(u => u.UserRole == UserRole.QuizMaster).Result.Id;
            command.ActorId = qmId;

            var quizrPackage = await command.Execute();

            var adminId = userCollection.FirstOrDefaultAsync(u => u.UserRole == UserRole.Admin).Result.Id;
            var createGameCommand = new CreateGameCommand(_unitOfWork, _bus)
            {
                ActorId = adminId,
                QuizId = quizrPackage.QuizRefs[0].Id,
                InviteCode = inviteCode,
                GameTitle = gameTitle
            };

            var game = await createGameCommand.Execute();

            var selectGameNotification = new SelectGameNotification(_unitOfWork, _bus)
            {
                ActorId = qmId,
                GameId = game.Id
            };
            await selectGameNotification.Execute();


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
            var gameRef = new GameRef
                {Id = game.Id, Title = game.Title, QuizTitle = game.QuizTitle, InviteCode = game.InviteCode};
            users.First(u => u.UserName == "Quiz master 1").GameRefs.Add(gameRef);
            users.First(u => u.UserName == "Quiz master 1").QuizRefs.Add(new QuizRef
            {
                Id = quiz.Id,
                Title = quiz.Title,
                GameRefs = new List<GameRef> {gameRef}
            });
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