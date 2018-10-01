using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Pubquiz.Persistence.Decorators;

namespace Pubquiz.Persistence.NoAction
{
    /// <summary>
    ///     Factory doesnt do anything with storage. Can be decorated with a memory repository to do the 'storage'
    /// </summary>
    public class NoActionUnitOfWork : UnitOfWorkBase
    {
        private readonly ILogger _logger;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="memoryCache"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="options"></param>
        public NoActionUnitOfWork(IMemoryCache memoryCache, ILoggerFactory loggerFactory, ICollectionOptions options)
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
        public override ICollection<T> GetCollection<T>()
        {
            if (Repositories.TryGetValue(typeof(T), out var repository))
            {
                return (ICollection<T>) repository;
            }

            var noActionRepository = new FlagAsDeletedDecorator<T>(MemoryCache,
                new FillDefaultValueDecorator<T>(MemoryCache,
                    new CacheDecorator<T>(MemoryCache, true,
                        new NoActionCollection<T>()), ActorId));
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

        public override void Commit()
        {
        }

        public override void Abort()
        {
            throw new NotImplementedException();
        }
    }
}