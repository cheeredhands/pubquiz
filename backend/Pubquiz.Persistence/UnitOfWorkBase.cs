﻿using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Pubquiz.Domain;

namespace Pubquiz.Persistence
{
    /// <summary>
    ///     Base class for unit of work implementations.
    /// </summary>
    public abstract class UnitOfWorkBase : IUnitOfWork
    {
        protected string ActorId { get; set; }
        protected readonly ILoggerFactory LoggerFactory;
        protected readonly bool LogTime;
        protected readonly IMemoryCache MemoryCache;

        /// <summary>
        /// </summary>
        /// <param name="memoryCache"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="options"></param>
        protected UnitOfWorkBase(IMemoryCache memoryCache, ILoggerFactory loggerFactory,
            ICollectionOptions options)
        {
            MemoryCache = memoryCache;
            LoggerFactory = loggerFactory;
            var logger = loggerFactory.CreateLogger(GetType());
            if (!options.TimeLoggingEnabled) return;
            logger.LogDebug(
                "Time logging is enabled in appsettings: logging time with the LogTimeDecorator will be started.");
            LogTime = true;
        }

        /// <inheritdoc />
        public abstract ICollection<T> GetCollection<T>() where T : Model, new();
    }
}