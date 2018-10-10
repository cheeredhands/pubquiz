using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Handlers;
using Pubquiz.Logic.Messages;
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
    public class InitializedTestBase
    {
        protected IUnitOfWork UnitOfWork;
        protected Game Game;
        protected Quiz Quiz;
        protected List<User> Users;
        protected List<Team> Teams;
        protected List<Question> QuestionsInQuiz;
        protected List<Question> OtherQuestions;
        protected IBus Bus;
        private ILoggerFactory _loggerFactory;
        private InMemorySubscriberStore _inMemorySubscriberStore;

        [TestInitialize]
        public void Initialize()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            _loggerFactory = new LoggerFactory();
            _loggerFactory.AddConsole();
            ICollectionOptions inMemoryCollectionOptions = new InMemoryDatabaseOptions();
            UnitOfWork = new NoActionUnitOfWork(memoryCache, _loggerFactory, inMemoryCollectionOptions);

            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            var userCollection = UnitOfWork.GetCollection<User>();
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var questionCollection = UnitOfWork.GetCollection<Question>();

            Users = TestUsers.GetUsers();
            Quiz = TestQuiz.GetQuiz();
            Game = TestGame.GetGame(Users.Where(u => u.UserName == "Quiz master 1").Select(u => u.Id), 0,
                Quiz.QuizSections[0].Id);

            Teams = TestTeams.GetTeams(teamCollection, Game.Id);
            Game.QuizId = Quiz.Id;
            Game.TeamIds = Teams.Select(t => t.Id).ToList();
            QuestionsInQuiz = TestQuiz.GetQuestions();
            OtherQuestions = new List<Question> {new Question(), new Question(), new Question()};
            Task.WaitAll(
                quizCollection.AddAsync(Quiz),
                QuestionsInQuiz.ToAsyncEnumerable().ForEachAsync(q => questionCollection.AddAsync(q)),
                OtherQuestions.ToAsyncEnumerable().ForEachAsync(q => questionCollection.AddAsync(q)),
                Teams.ToAsyncEnumerable().ForEachAsync(t => teamCollection.AddAsync(t)),
                Users.ToAsyncEnumerable().ForEachAsync(u => userCollection.AddAsync(u)),
                gameCollection.AddAsync(Game));

            // set up bus
            var activator = new BuiltinHandlerActivator();
            activator.Register((bus, messageContext) => new ScoringHandler(UnitOfWork, bus));
            activator.Register(() => new ClientNotificationHandler(_loggerFactory, null));

            // needed so the inmemory subscription store will be centralized
            _inMemorySubscriberStore = new InMemorySubscriberStore();

            Configure.With(activator).Logging(l => l.ColoredConsole())
                .Transport(t => t.UseInMemoryTransport(new InMemNetwork(true), "Messages"))
                .Routing(r => r.TypeBased().MapAssemblyOf<TeamMembersChanged>("Messages"))
                .Subscriptions(s => s.StoreInMemory(_inMemorySubscriberStore))
                .Start();

            Bus = activator.Bus;
            Bus.SubscribeByScanningForHandlers(Assembly.Load("Pubquiz.Logic"));
        }
    }
}