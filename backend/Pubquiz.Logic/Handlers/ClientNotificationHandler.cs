using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pubquiz.Logic.Hubs;
using Pubquiz.Logic.Messages;
using Rebus.Handlers;

namespace Pubquiz.Logic.Handlers
{
    public class ClientNotificationHandler :
        IHandleMessages<AnswerScored>, IHandleMessages<InteractionResponseAdded>, IHandleMessages<TeamMembersChanged>,
        IHandleMessages<ErrorOccurred>, IHandleMessages<TeamRegistered>, IHandleMessages<GameStateChanged>,
        IHandleMessages<TeamNameUpdated>
    {
        private readonly GameHub _gameHub;
        private readonly ILogger _logger;

        public ClientNotificationHandler(ILoggerFactory loggerFactory, GameHub gameHub)
        {
            _gameHub = gameHub;
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

        public Task Handle(TeamMembersChanged message)
        {
            return _gameHub.TeamMembersChangedAsync(message);
        }

        public Task Handle(ErrorOccurred message) => Task.Run(() =>
        {
            // notify clients via hub
        });

        public Task Handle(TeamRegistered message)
        {
            return _gameHub?.TeamRegisteredAsync(message);
        }

        public Task Handle(GameStateChanged message)
        {
            return _gameHub?.GameStateChangedAsync(message);
        }

        public Task Handle(TeamNameUpdated message)
        {
            return _gameHub?.TeamNameUpdatedAsync(message);
        }
    }
}