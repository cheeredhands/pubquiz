using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pubquiz.Logic.Messages;
using Rebus.Handlers;

namespace Pubquiz.Logic.Handlers
{
    public class ClientNotificationHandler :
        IHandleMessages<AnswerScored>, IHandleMessages<InteractionResponseAdded>, IHandleMessages<TeamMembersChanged>
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
        }

        public async Task Handle(TeamMembersChanged message)
        {
            _logger.LogInformation("test!!!!!!!!!!!!!!");
        }
    }
}