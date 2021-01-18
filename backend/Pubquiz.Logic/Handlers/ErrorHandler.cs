using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Pubquiz.Logic.Messages;

namespace Pubquiz.Logic.Handlers
{
    public class ErrorHandler : INotificationHandler<ErrorOccurred>
    {
        private readonly ILogger _logger;

        public ErrorHandler(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ErrorHandler>();
        }

        public Task Handle(ErrorOccurred message, CancellationToken cancellationToken) => Task.Run(() =>
        {
            _logger.LogError((int)message.DomainException.ResultCode, message.DomainException,
                "An error occurred while handling a message.");
        });
    }
}