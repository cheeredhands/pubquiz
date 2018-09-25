using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Pubquiz.Repository.Decorators;

namespace Pubquiz.Repository.NoAction
{
    /// <summary>
    ///     Factory doesnt do anything with storage. Can be decorated with a memory repository to do the 'storage'
    /// </summary>
    public class NoActionFactory : RepositoryFactoryBase
    {
        private readonly ILogger _logger;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="memoryCache"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="options"></param>
        public NoActionFactory(IMemoryCache memoryCache, ILoggerFactory loggerFactory, IRepositoryOptions options)
            : base(memoryCache, loggerFactory, options)
        {
            Repositories = new ConcurrentDictionary<Type, object>();
            _logger = loggerFactory.CreateLogger(GetType());
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
            var noActionRepository = new FlagAsDeletedDecorator<T>(MemoryCache,
                new FillDefaultValueDecorator<T>(MemoryCache,
                    new CacheDecorator<T>(MemoryCache, true,
                        new NoActionRepository<T>()), ActorId));
            if (LogTime)
            {
                var timeLoggedNoActionRepository =
                    new LogTimeDecorator<T>(MemoryCache, noActionRepository, _logger);
                Repositories.TryAdd(typeof(T), timeLoggedNoActionRepository);
            }
            else
            {
                Repositories.TryAdd(typeof(T), noActionRepository);
            }
            Repositories.TryAdd(typeof(T), noActionRepository);
            return noActionRepository;
        }
    }
}