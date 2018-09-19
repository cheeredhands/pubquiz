using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Pubquiz.Repository.Decorators;

namespace Pubquiz.Repository.Mongo
{
    /// <summary>
    ///     Factory for mongo database
    /// </summary>
    public class MongoFactory : RepositoryFactoryBase
    {
        private readonly ILogger _logger;
        private readonly IMongoDatabase _mongoDatabase;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="memoryCache"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="options"></param>
        public MongoFactory(IMemoryCache memoryCache, ILoggerFactory loggerFactory,
            IRepositoryOptions options)
            : base(memoryCache, loggerFactory, options)
        {
            if (!(options is IMongoDatabaseOptions mongoOptions)) throw new Exception("Options should be of type IMongoDatabaseOptions");
            _logger = loggerFactory.CreateLogger(GetType());
            try
            {
                MongoClient client;
                try
                {
                    var mongoClientSettings = new MongoClientSettings
                    {
                        Server = MongoServerAddress.Parse(mongoOptions.ConnectionString)
                    };
                    client = new MongoClient(mongoClientSettings);
                }
                catch
                {
                    client = new MongoClient(mongoOptions.ConnectionString);
                }

                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (string.IsNullOrWhiteSpace(environment))
                {
                    environment = "Development";
                }
                _mongoDatabase = client.GetDatabase($"{mongoOptions.DatabaseName}-{environment}");
                Repositories = new ConcurrentDictionary<Type, object>();
            }
            catch (Exception exception)
            {
                _logger.LogCritical(
                    $"Error while connecting to {mongoOptions.ConnectionString}.", exception);
                throw;
            }
        }

        /// <summary>
        ///     Repositories
        /// </summary>
        public ConcurrentDictionary<Type, object> Repositories { get; set; }

        /// <inheritdoc />
        public override IRepository<T> GetRepository<T>()
        {
            if (Repositories.TryGetValue(typeof(T), out var repository))
            {
                return (IRepository<T>)repository;
            }
            var mongoRepository = new FlagAsDeletedDecorator<T>(MemoryCache,
                new FillDefaultValueDecorator<T>(MemoryCache,
                    new CacheDecorator<T>(MemoryCache, false,
                        new MongoRepository<T>(LoggerFactory, _mongoDatabase)), ActorId));
            if (LogTime)
            {
                var timeLoggedMongoRepository = new LogTimeDecorator<T>(MemoryCache, mongoRepository, _logger);
                Repositories.TryAdd(typeof(T), timeLoggedMongoRepository);
            }
            else
            {
                Repositories.TryAdd(typeof(T), mongoRepository);
            }
            return mongoRepository;
        }
    }
}