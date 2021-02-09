using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Handlers;
using Pubquiz.Logic.Hubs;
using Pubquiz.Logic.Messages;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Pubquiz.Persistence.NoAction;

namespace Pubquiz.Domain.Tests
{
    [TestClass]
    public class InitializedTestBase
    {
        protected IUnitOfWork UnitOfWork;
        protected Game Game;
        protected Quiz Quiz;
        protected List<User> Users;
        protected List<Team> Teams;
        protected List<QuizItem> QuestionsInQuiz;
        protected List<QuizItem> OtherQuestions;
        protected IMediator Mediator;
        protected ILoggerFactory LoggerFactory;
        protected QuizrSettings QuizrSettings;

        [TestInitialize]
        public void Initialize()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // var memoryCache = new MemoryCache(new MemoryCacheOptions());
            //
            // ICollectionOptions inMemoryCollectionOptions = new InMemoryDatabaseOptions();
            // UnitOfWork = new NoActionUnitOfWork(memoryCache, LoggerFactory, inMemoryCollectionOptions);

            var services = new ServiceCollection();

            var serviceProvider = services
                .AddLogging(builder => builder.AddConsole())
                .AddMemoryCache()
                .AddSingleton(_ => new QuizrSettings
                {
                    BaseUrl = "https://localhost:5001",
                    WebRootPath = "",
                    ContentPath = "quiz"
                })
                .AddSingleton<ICollectionOptions, InMemoryDatabaseOptions>()
                .AddScoped<IUnitOfWork, NoActionUnitOfWork>()
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>))
                .AddTransient<IRequestPreProcessor<IRequest>, ValidationPreProcessor<IRequest>>()
                .AddMediatR(typeof(AnswerScored))
                .AddScoped(_ => new Mock<IHubContext<GameHub, IGameHub>>().Object)
                .BuildServiceProvider();


            LoggerFactory = serviceProvider.GetService<ILoggerFactory>();
            UnitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            Mediator = serviceProvider.GetRequiredService<IMediator>();
            QuizrSettings = serviceProvider.GetRequiredService<QuizrSettings>();


            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            var userCollection = UnitOfWork.GetCollection<User>();
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var questionCollection = UnitOfWork.GetCollection<QuizItem>();

            Users = TestUsers.GetUsers();
            Quiz = TestQuiz.GetQuiz();
            Game = TestGame.GetGame(Users.Where(u => u.UserName == "Quiz master 1").Select(u => u.Id), Quiz);
            var gameRef = new GameRef
                {Id = Game.Id, Title = Game.Title, QuizTitle = Game.QuizTitle, InviteCode = Game.InviteCode};
            Users.First(u => u.UserName == "Quiz master 1").GameRefs.Add(gameRef);
            Users.First(u => u.UserName == "Quiz master 1").QuizRefs.Add(new QuizRef
                {Id = Quiz.Id, Title = Quiz.Title, GameRefs = new List<GameRef> {gameRef}});
            Teams = TestTeams.GetTeams(teamCollection, Game.Id);
            Game.QuizId = Quiz.Id;
            Game.TeamIds = Teams.Select(t => t.Id).ToList();
            QuestionsInQuiz = TestQuiz.GetQuizItems();
            OtherQuestions = new List<QuizItem> {new QuizItem(), new QuizItem(), new QuizItem()};
            Task.WaitAll(
                quizCollection.AddAsync(Quiz),
                QuestionsInQuiz.ToAsyncEnumerable().ForEachAsync(q => questionCollection.AddAsync(q)),
                OtherQuestions.ToAsyncEnumerable().ForEachAsync(q => questionCollection.AddAsync(q)),
                Teams.ToAsyncEnumerable().ForEachAsync(t => teamCollection.AddAsync(t)),
                Users.ToAsyncEnumerable().ForEachAsync(u => userCollection.AddAsync(u)),
                gameCollection.AddAsync(Game));
        }
    }
}