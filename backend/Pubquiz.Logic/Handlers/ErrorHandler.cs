using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using ErrorOccurred = Pubquiz.Logic.Messages.ErrorOccurred;

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
            _logger.LogError((int)message.DomainException.ResultCode, message.DomainException,
                "An error occurred while handling a message.");
        });
    }
}