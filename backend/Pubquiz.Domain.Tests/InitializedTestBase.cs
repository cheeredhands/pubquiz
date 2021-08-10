using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Handlers;
using Pubquiz.Logic.Messages;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Pubquiz.Persistence.NoAction;
using SimpleInjector;
using SimpleInjector.Lifestyles;

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
        private readonly Container _container = new SimpleInjector.Container();
        private Scope _scope;

        [TestCleanup]
        public void TestCleanup()
        {
            _scope?.Dispose();
        }

        [TestInitialize]
        public void Initialize()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // var memoryCache = new MemoryCache(new MemoryCacheOptions());
            //
            // ICollectionOptions inMemoryCollectionOptions = new InMemoryDatabaseOptions();
            // UnitOfWork = new NoActionUnitOfWork(memoryCache, LoggerFactory, inMemoryCollectionOptions);

            var services = new ServiceCollection();
            Directory.Delete("quiz", true);
            Directory.CreateDirectory("quiz");
            services
                .AddLogging(builder => builder.AddConsole())
                .AddMemoryCache()
                .AddSingleton(_ => new QuizrSettings
                {
                    BaseUrl = "https://localhost:5001",
                    WebRootPath = "",
                    ContentPath = "quiz"
                })
                .AddSingleton<ICollectionOptions, InMemoryDatabaseOptions>()
                .AddScoped<IUnitOfWork, NoActionUnitOfWork>();

            AddDI(services);
            AddMediatr(_container, typeof(TeamRegistered).Assembly);
            _container.Collection.Register(typeof(IRequestPreProcessor<>),
                new[]
                {
                    typeof(RequestValidationPreProcessor<>)
                });

            var serviceProvider = services.BuildServiceProvider();
            serviceProvider.UseSimpleInjector(_container);

            _container.Verify();

            _scope = AsyncScopedLifestyle.BeginScope(_container);

            LoggerFactory = _container.GetInstance<ILoggerFactory>();
            UnitOfWork = _container.GetInstance<IUnitOfWork>();
            Mediator = _container.GetInstance<IMediator>();
            QuizrSettings = _container.GetInstance<QuizrSettings>();

            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            var userCollection = UnitOfWork.GetCollection<User>();
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var questionCollection = UnitOfWork.GetCollection<QuizItem>();

            Users = TestUsers.GetUsers();
            Quiz = TestQuiz.GetQuiz();
            Game = TestGame.GetGame(Users.Where(u => u.UserName == "Quiz master 1").Select(u => u.Id), Quiz);
            Users.First(u => u.UserName == "Quiz master 1").GameIds.Add(Game.Id);
            Users.First(u => u.UserName == "Quiz master 1").QuizIds.Add(Quiz.Id);
            Teams = TestTeams.GetTeams(teamCollection, Game.Id);
            Game.QuizId = Quiz.Id;
            Game.TeamIds = Teams.Select(t => t.Id).ToList();
            QuestionsInQuiz = TestQuiz.GetQuizItems();
            OtherQuestions = new List<QuizItem> { new QuizItem(), new QuizItem(), new QuizItem() };
            Task.WaitAll(
                quizCollection.AddAsync(Quiz),
                QuestionsInQuiz.ToAsyncEnumerable().ForEachAsync(q => questionCollection.AddAsync(q)),
                OtherQuestions.ToAsyncEnumerable().ForEachAsync(q => questionCollection.AddAsync(q)),
                Teams.ToAsyncEnumerable().ForEachAsync(t => teamCollection.AddAsync(t)),
                Users.ToAsyncEnumerable().ForEachAsync(u => userCollection.AddAsync(u)),
                gameCollection.AddAsync(Game));
        }

        private void AddMediatr(Container container, params Assembly[] assemblies)
        {
            var allAssemblies = GetAssemblies(assemblies);

            container.RegisterSingleton<IMediator, Mediator>();
            container.Register(typeof(IRequestHandler<,>), assemblies);

            RegisterHandlers(container, typeof(INotificationHandler<>), allAssemblies);
            //RegisterHandlers(container, typeof(IRequestExceptionAction<,>), allAssemblies);
            //RegisterHandlers(container, typeof(IRequestExceptionHandler<,,>), allAssemblies);

            //Pipeline
            container.Collection.Register(typeof(IPipelineBehavior<,>), new[]
            {
                //typeof(RequestExceptionProcessorBehavior<,>),
                //typeof(RequestExceptionActionProcessorBehavior<,>),
                typeof(RequestPreProcessorBehavior<,>),
                //typeof(RequestPostProcessorBehavior<,>)
            });

            container.Register(() => new ServiceFactory(container.GetInstance), Lifestyle.Singleton);
        }

        private static void RegisterHandlers(Container container, Type collectionType, Assembly[] assemblies)
        {
            // we have to do this because by default, generic type definitions (such as the Constrained Notification Handler) won't be registered
            var handlerTypes = container.GetTypesToRegister(collectionType, assemblies, new TypesToRegisterOptions
            {
                IncludeGenericTypeDefinitions = true,
                IncludeComposites = false,
            });

            container.Collection.Register(collectionType, handlerTypes);
        }

        private static Assembly[] GetAssemblies(IEnumerable<Assembly> assemblies)
        {
            var allAssemblies = new List<Assembly> { typeof(IMediator).GetTypeInfo().Assembly };
            allAssemblies.AddRange(assemblies);

            return allAssemblies.ToArray();
        }

        private void AddDI(ServiceCollection services)
        {
            // Sets up the basic configuration that for integrating Simple Injector with
            // ASP.NET Core by setting the DefaultScopedLifestyle, and setting up auto
            // cross wiring.
            services.AddSimpleInjector(_container, options =>
            {
                // AddAspNetCore() wraps web requests in a Simple Injector scope and
                // allows request-scoped framework services to be resolved.
                //options.AddAspNetCore()

                // Ensure activation of a specific framework type to be created by
                // Simple Injector instead of the built-in configuration system.
                // All calls are optional. You can enable what you need. For instance,
                // ViewComponents, PageModels, and TagHelpers are not needed when you
                // build a Web API.
                //.AddControllerActivation();
                //.AddViewComponentActivation()
                //.AddPageModelActivation()
                //.AddTagHelperActivation();

                // Optionally, allow application components to depend on the non-generic
                // ILogger (Microsoft.Extensions.Logging) or IStringLocalizer
                // (Microsoft.Extensions.Localization) abstractions.
                options.AddLogging();
                //options.AddLocalization();
            });
        }
    }
}