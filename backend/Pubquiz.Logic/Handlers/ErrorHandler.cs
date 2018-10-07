using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pubquiz.Logic.Messages;
using Rebus.Handlers;

namespace Pubquiz.Logic.Handlers
{
    public class ErrorHandler : IHandleMessages<ErrorOccurred>
    {
        private readonly ILogger _logger;

        public ErrorHandler(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ErrorHandler>();
        }

        public Task Handle(ErrorOccurred message) => Task.Run(() =>
        {
            _logger.LogError(message.DomainException.ErrorCode, message.DomainException,
                "An error occurred while handling a message.");
        });
    }
}