using System;
using Microsoft.Extensions.Logging;
using Rebus.Logging;

namespace Pubquiz.WebApi.Helpers
{
    public class MsLoggerFactoryAdapter : AbstractRebusLoggerFactory
    {
        private readonly ILoggerFactory _logger;

        public MsLoggerFactoryAdapter(ILoggerFactory logger)
        {
            _logger = logger;
        }

        protected override ILog GetLogger(Type type)
        {
            return new MsLoggerAdapter(_logger.CreateLogger(type));
        }
    }
}