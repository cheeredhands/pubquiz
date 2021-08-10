using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Requests.Commands;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Tools
{
    public class TestSeeder
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly QuizrSettings _quizrSettings;

        public TestSeeder(IUnitOfWork unitOfWork, ILoggerFactory loggerFactory, IMediator mediator,
            QuizrSettings quizrSettings)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _quizrSettings = quizrSettings;
            _logger = loggerFactory.CreateLogger<TestSeeder>();
        }

        public void SeedSeedSet(string mediaBaseUrl)
        {
            _logger.LogInformation("Seeding the seed set");
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
            users.First(u => u.UserName == "Quiz master 1").QuizIds.Add(quiz.Id);
            users.First(u => u.UserName == "Quiz master 1").CurrentGameId = game.Id;
            var teams = SeedTeams.GetTeams(teamCollection, game.Id);
            var teamUsers = SeedTeams.GetUsersFromTeams(teams);
            foreach (var team in teams)
            {
                _logger.LogInformation("{Name}: {RecoveryCode}", team.Name, team.RecoveryCode);
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
            var userCollection = _unitOfWork.GetCollection<User>();
            var qmId = userCollection.FirstOrDefaultAsync(u => u.UserRole == UserRole.QuizMaster).Result.Id;
            var command = new ImportZippedExcelQuizCommand {ActorId = qmId, FileName = fileName, FileStream = stream};

            var quizrPackage = await _mediator.Send(command);

            //ar adminId = userCollection.FirstOrDefaultAsync(u => u.UserRole == UserRole.Admin).Result.Id;

            var createGameCommand = new CreateGameCommand
            {
                ActorId = qmId,
                QuizId = quizrPackage.QuizViewModels[0].Id,
                InviteCode = inviteCode,
                GameTitle = gameTitle
            };

            await _mediator.Send(createGameCommand);
            
            // var selectGameNotification = new SelectGameNotification(_unitOfWork, _bus)
            // {
            //     ActorId = qmId,
            //     GameId = game.Id
            // };
            // await selectGameNotification.Execute();
        }

        public void SeedTestSet()
        {
            _logger.LogInformation("Seeding test set");
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
            users.First(u => u.UserName == "Quiz master 1").QuizIds.Add(quiz.Id);
            users.First(u => u.UserName == "Quiz master 1").CurrentGameId = game.Id;
            var teams = TestTeams.GetTeams(teamCollection, game.Id);
            foreach (var team in teams)
            {
                _logger.LogInformation("{Name}: {RecoveryCode}", team.Name, team.RecoveryCode);
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