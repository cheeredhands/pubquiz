using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pubquiz.Logic.Messages;
using Rebus.Handlers;

namespace Pubquiz.Logic.Handlers
{
    public class ClientNotificationHandler :
        IHandleMessages<AnswerScored>, IHandleMessages<InteractionResponseAdded>, IHandleMessages<TeamMembersChanged>,
        IHandleMessages<ErrorOccurred>
    {
        private readonly ILogger _logger;

        public ClientNotificationHandler(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ClientNotificationHandler>();
        }

        public async Task Handle(AnswerScored message)
        {
            // notify clients via hub
        }

        public async Task Handle(InteractionResponseAdded message)
        {
            // notify clients via hub
            // something like:
            // var sendMessage = new {Code = 100, message.TeamId, message.TeamName, message.QuestionId, message.Response};
        }

        public async Task Handle(TeamMembersChanged message)
        {
            _logger.LogInformation("test!!!!!!!!!!!!!!");
        }

        public async Task Handle(ErrorOccurred message)
        {
            // notify clients via hub
        }
    }
}