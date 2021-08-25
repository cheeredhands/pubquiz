using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Pubquiz.Persistence.Decorators;

namespace Pubquiz.Persistence.InMemory
{
    /// <summary>
    ///     Unit of work doesn't do anything with storage. Can be decorated with a memory cache collection to do the 'storage'
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
            Collections = new ConcurrentDictionary<Type, object>();
            _logger = loggerFactory.CreateLogger(GetType());
        }

        /// <summary>
        ///     Collections
        /// </summary>
        private ConcurrentDictionary<Type, object> Collections { get; set; }

        /// <inheritdoc />
        public override ICollection<T> GetCollection<T>()
        {
            if (Collections.TryGetValue(typeof(T), out var collection))
            {
                return (ICollection<T>) collection;
            }

            var noActionCollection = new FlagAsDeletedDecorator<T>(MemoryCache,
                new FillDefaultValueDecorator<T>(MemoryCache,
                    new MemoryCacheDecorator<T>(MemoryCache, true,
                        new NoActionCollection<T>()), ActorId));
            if (LogTime)
            {
                var timeLoggedNoActionCollection =
                    new LogTimeDecorator<T>(MemoryCache, noActionCollection, _logger);
                Collections.TryAdd(typeof(T), timeLoggedNoActionCollection);
            }
            else
            {
                Collections.TryAdd(typeof(T), noActionCollection);
            }

            Collections.TryAdd(typeof(T), noActionCollection);
            return noActionCollection;
        }
    }
}