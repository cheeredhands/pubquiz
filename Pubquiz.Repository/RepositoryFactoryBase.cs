using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Pubquiz.Repository
{
    /// <summary>
    ///     Base class for factories
    /// </summary>
    public abstract class RepositoryFactoryBase : IRepositoryFactory
    {
        public Guid? ActorId { get; set; }
        protected readonly ILoggerFactory LoggerFactory;
        protected readonly bool LogTime;
        protected readonly IMemoryCache MemoryCache;

        /// <summary>
        /// </summary>
        /// <param name="memoryCache"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="options"></param>
        protected RepositoryFactoryBase(IMemoryCache memoryCache, ILoggerFactory loggerFactory, IRepositoryOptions options)
        {
            MemoryCache = memoryCache;
            LoggerFactory = loggerFactory;
            var logger = loggerFactory.CreateLogger(GetType());
            if (!options.TimeLoggingEnabled) return;
            logger.LogDebug("Time logging is enabled in appsettings: logging time with the LogTimeDecorator will be started.");
            LogTime = true;
        }

        /// <inheritdoc />
        public abstract IRepository<T> GetRepository<T>() where T : Model, new();
    }
}