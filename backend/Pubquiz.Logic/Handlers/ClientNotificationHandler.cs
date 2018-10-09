using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pubquiz.Logic.Messages;
using Rebus.Handlers;

namespace Pubquiz.Logic.Handlers
{
    public class ClientNotificationHandler :
        IHandleMessages<AnswerScored>, IHandleMessages<InteractionResponseAdded>, IHandleMessages<TeamMembersChanged>,
        IHandleMessages<ErrorOccurred>, IHandleMessages<TeamRegistered>
    {
        private readonly ILogger _logger;

        public ClientNotificationHandler(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ClientNotificationHandler>();
        }

        public Task Handle(AnswerScored message) => Task.Run(() =>
        {
            // notify clients via hub
        });

        public Task Handle(InteractionResponseAdded message) => Task.Run(() =>
        {
            // notify clients via hub
            // something like:
            // var sendMessage = new {Code = 100, message.TeamId, message.TeamName, message.QuestionId, message.Response};
        });

        public Task Handle(TeamMembersChanged message) => Task.Run(() =>
        {
            // notify clients via hub
            _logger.LogInformation("Handling TeamMembersChanged message");
        });

        public Task Handle(ErrorOccurred message) => Task.Run(() =>
        {
            // notify clients via hub
        });

        public Task Handle(TeamRegistered message) => Task.Run(() =>
        {
            // notify clients via hub
        });
    }
}