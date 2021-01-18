using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Pubquiz.Logic.Hubs;
using Pubquiz.Logic.Messages;
using Pubquiz.Logic.Tools;

namespace Pubquiz.Logic.Handlers
{
    public class ClientNotificationHandler :
        INotificationHandler<AnswerScored>, INotificationHandler<TeamMembersChanged>,
        INotificationHandler<ErrorOccurred>, INotificationHandler<TeamRegistered>, INotificationHandler<QmTeamRegistered>,
        INotificationHandler<GameStateChanged>, INotificationHandler<TeamNameUpdated>, INotificationHandler<TeamLoggedOut>,
        INotificationHandler<UserLoggedOut>, INotificationHandler<TeamDeleted>, INotificationHandler<ItemNavigated>,
        INotificationHandler<InteractionResponseAdded>, INotificationHandler<TeamConnectionChanged>, INotificationHandler<GameSelected>
    {
        private readonly IHubContext<GameHub, IGameHub> _gameHubContext;
        private readonly ILogger _logger;

        public ClientNotificationHandler(ILoggerFactory loggerFactory, IHubContext<GameHub, IGameHub> gameHubContext)
        {
            _gameHubContext = gameHubContext;
            _logger = loggerFactory.CreateLogger<ClientNotificationHandler>();
        }

        public async Task Handle(AnswerScored message, CancellationToken cancellationToken)
        {
            var quizMasterGroupId = Helpers.GetQuizMasterGroupId(message.GameId);

            // notify quiz master 
            await _gameHubContext.Clients.Group(quizMasterGroupId).AnswerScored(message);
        }

        public async Task Handle(InteractionResponseAdded message, CancellationToken cancellationToken)
        {
            var quizMasterGroupId = Helpers.GetQuizMasterGroupId(message.GameId);

            // notify quiz master 
            await _gameHubContext.Clients.Group(quizMasterGroupId).InteractionResponseAdded(message);
        }

        public async Task Handle(TeamMembersChanged message, CancellationToken cancellationToken)
        {
            var teamGroupId = Helpers.GetTeamsGroupId(message.GameId);
            var quizMasterGroupId = Helpers.GetQuizMasterGroupId(message.GameId);

            // notify quiz master 
            await _gameHubContext.Clients.Group(quizMasterGroupId).TeamMembersChanged(message);

            // notify teams
            await _gameHubContext.Clients.Group(teamGroupId).TeamMembersChanged(message);
        }

        public Task Handle(ErrorOccurred message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task Handle(QmTeamRegistered message, CancellationToken cancellationToken)
        {
            var quizMasterGroupId = Helpers.GetQuizMasterGroupId(message.GameId);

            // notify quiz master 
            await _gameHubContext.Clients.Group(quizMasterGroupId).QmTeamRegistered(message);
        }

        public async Task Handle(TeamRegistered message, CancellationToken cancellationToken)
        {
            var teamGroupId = Helpers.GetTeamsGroupId(message.GameId);

            // notify teams
            await _gameHubContext.Clients.Group(teamGroupId).TeamRegistered(message);
        }
        
        public async Task Handle(TeamConnectionChanged message, CancellationToken cancellationToken)
        {
            var teamGroupId = Helpers.GetTeamsGroupId(message.GameId);
            var quizMasterGroupId = Helpers.GetQuizMasterGroupId(message.GameId);

            // notify quiz master 
            await _gameHubContext.Clients.Group(quizMasterGroupId).TeamConnectionChanged(message);

            // notify teams
            await _gameHubContext.Clients.Group(teamGroupId).TeamConnectionChanged(message);
        }
        
        public async Task Handle(TeamLoggedOut message, CancellationToken cancellationToken)
        {
            var teamGroupId = Helpers.GetTeamsGroupId(message.GameId);
            var quizMasterGroupId = Helpers.GetQuizMasterGroupId(message.GameId);

            // notify quiz master 
            await _gameHubContext.Clients.Group(quizMasterGroupId).TeamLoggedOut(message);

            // notify teams
            await _gameHubContext.Clients.Group(teamGroupId).TeamLoggedOut(message);
        }

        public async Task Handle(UserLoggedOut message, CancellationToken cancellationToken)
        {
            var teamGroupId = Helpers.GetTeamsGroupId(message.GameId);
            var quizMasterGroupId = Helpers.GetQuizMasterGroupId(message.GameId);

            // notify quiz master 
            await _gameHubContext.Clients.Group(quizMasterGroupId).UserLoggedOut(message);

            // notify teams
            await _gameHubContext.Clients.Group(teamGroupId).UserLoggedOut(message);
        }

        public async Task Handle(GameStateChanged message, CancellationToken cancellationToken)
        {
            var teamGroupId = Helpers.GetTeamsGroupId(message.GameId);
            var quizMasterGroupId = Helpers.GetQuizMasterGroupId(message.GameId);

            // notify quiz master 
            await _gameHubContext.Clients.Group(quizMasterGroupId).GameStateChanged(message);

            // notify teams
            await _gameHubContext.Clients.Group(teamGroupId).GameStateChanged(message);
        }

        public async Task Handle(TeamNameUpdated message, CancellationToken cancellationToken)
        {
            var teamGroupId = Helpers.GetTeamsGroupId(message.GameId);
            var quizMasterGroupId = Helpers.GetQuizMasterGroupId(message.GameId);

            // notify quiz master 
            await _gameHubContext.Clients.Group(quizMasterGroupId).TeamNameUpdated(message);

            // notify other teams
            await _gameHubContext.Clients.Group(teamGroupId).TeamNameUpdated(message);
        }

        public async Task Handle(TeamDeleted message, CancellationToken cancellationToken)
        {
            var teamGroupId = Helpers.GetTeamsGroupId(message.GameId);
            var quizMasterGroupId = Helpers.GetQuizMasterGroupId(message.GameId);

            // notify quiz master 
            await _gameHubContext.Clients.Group(quizMasterGroupId).TeamDeleted(message);

            // notify other teams
            await _gameHubContext.Clients.Group(teamGroupId).TeamDeleted(message);
        }

        public async Task Handle(ItemNavigated message, CancellationToken cancellationToken)
        {
            var teamGroupId = Helpers.GetTeamsGroupId(message.GameId);
            var quizMasterGroupId = Helpers.GetQuizMasterGroupId(message.GameId);

            // notify quiz master 
            await _gameHubContext.Clients.Group(quizMasterGroupId).ItemNavigated(message);

            // notify other teams
            await _gameHubContext.Clients.Group(teamGroupId).ItemNavigated(message);
        }

        public async Task Handle(GameSelected message, CancellationToken cancellationToken)
        {
            var quizMasterGroupId = Helpers.GetQuizMasterGroupId(message.GameId);

            // notify quiz master 
            await _gameHubContext.Clients.Group(quizMasterGroupId).GameSelected(message);
        }
    }
}