using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Pubquiz.Logic.Hubs;
using Pubquiz.Logic.Messages;
using Pubquiz.Logic.Tools;
using Rebus.Handlers;

namespace Pubquiz.Logic.Handlers
{
    public class ClientNotificationHandler :
        IHandleMessages<AnswerScored>, IHandleMessages<InteractionResponseAdded>, IHandleMessages<TeamMembersChanged>,
        IHandleMessages<ErrorOccurred>, IHandleMessages<TeamRegistered>, IHandleMessages<GameStateChanged>,
        IHandleMessages<TeamNameUpdated>, IHandleMessages<TeamLoggedOut>, IHandleMessages<UserLoggedOut>,
        IHandleMessages<TeamDeleted>, IHandleMessages<ItemNavigated>
    {
        private readonly IHubContext<GameHub, IGameHub> _gameHubContext;
        private readonly ILogger _logger;

        public ClientNotificationHandler(ILoggerFactory loggerFactory, IHubContext<GameHub, IGameHub> gameHubContext)
        {
            _gameHubContext = gameHubContext;
            _logger = loggerFactory.CreateLogger<ClientNotificationHandler>();
        }

        public async Task Handle(AnswerScored message)
        {
            // todo implement
            await Task.CompletedTask;
        }

        public async Task Handle(InteractionResponseAdded message)
        {
            // todo implement
            await Task.CompletedTask;
            // notify clients via hub
            // something like:
            // var sendMessage = new {Code = 100, message.TeamId, message.TeamName, message.QuestionId, message.Response};
        }

        public async Task Handle(TeamMembersChanged message)
        {
            var teamGroupId = Helpers.GetTeamsGroupId(message.GameId);
            var quizMasterGroupId = Helpers.GetQuizMasterGroupId(message.GameId);

            // notify quiz master 
            await _gameHubContext.Clients.Group(quizMasterGroupId).TeamMembersChanged(message);

            // notify teams
            await _gameHubContext.Clients.Group(teamGroupId).TeamMembersChanged(message);
        }

        public async Task Handle(ErrorOccurred message)
        {
            await Task.CompletedTask;
        }

        public async Task Handle(TeamRegistered message)
        {
            var teamGroupId = Helpers.GetTeamsGroupId(message.GameId);
            var quizMasterGroupId = Helpers.GetQuizMasterGroupId(message.GameId);

            // notify quiz master 
            await _gameHubContext.Clients.Group(quizMasterGroupId).TeamRegistered(message);

            // notify teams
            await _gameHubContext.Clients.Group(teamGroupId).TeamRegistered(message);
        }

        public async Task Handle(TeamLoggedOut message)
        {
            var teamGroupId = Helpers.GetTeamsGroupId(message.GameId);
            var quizMasterGroupId = Helpers.GetQuizMasterGroupId(message.GameId);

            // notify quiz master 
            await _gameHubContext.Clients.Group(quizMasterGroupId).TeamLoggedOut(message);

            // notify teams
            await _gameHubContext.Clients.Group(teamGroupId).TeamLoggedOut(message);
        }

        public async Task Handle(UserLoggedOut message)
        {
            var teamGroupId = Helpers.GetTeamsGroupId(message.GameId);
            var quizMasterGroupId = Helpers.GetQuizMasterGroupId(message.GameId);

            // notify quiz master 
            await _gameHubContext.Clients.Group(quizMasterGroupId).UserLoggedOut(message);

            // notify teams
            await _gameHubContext.Clients.Group(teamGroupId).UserLoggedOut(message);
        }

        public async Task Handle(GameStateChanged message)
        {
            var teamGroupId = Helpers.GetTeamsGroupId(message.GameId);
            var quizMasterGroupId = Helpers.GetQuizMasterGroupId(message.GameId);

            // notify quiz master 
            await _gameHubContext.Clients.Group(quizMasterGroupId).GameStateChanged(message);

            // notify teams
            await _gameHubContext.Clients.Group(teamGroupId).GameStateChanged(message);
        }

        public async Task Handle(TeamNameUpdated message)
        {
            var teamGroupId = Helpers.GetTeamsGroupId(message.GameId);
            var quizMasterGroupId = Helpers.GetQuizMasterGroupId(message.GameId);

            // notify quiz master 
            await _gameHubContext.Clients.Group(quizMasterGroupId).TeamNameUpdated(message);

            // notify other teams
            await _gameHubContext.Clients.Group(teamGroupId).TeamNameUpdated(message);
        }

        public async Task Handle(TeamDeleted message)
        {
            var teamGroupId = Helpers.GetTeamsGroupId(message.GameId);
            var quizMasterGroupId = Helpers.GetQuizMasterGroupId(message.GameId);

            // notify quiz master 
            await _gameHubContext.Clients.Group(quizMasterGroupId).TeamDeleted(message);

            // notify other teams
            await _gameHubContext.Clients.Group(teamGroupId).TeamDeleted(message);
        }

        public async Task Handle(ItemNavigated message)
        {
            var teamGroupId = Helpers.GetTeamsGroupId(message.GameId);
            var quizMasterGroupId = Helpers.GetQuizMasterGroupId(message.GameId);

            // notify quiz master 
            await _gameHubContext.Clients.Group(quizMasterGroupId).ItemNavigated(message);

            // notify other teams
            await _gameHubContext.Clients.Group(teamGroupId).ItemNavigated(message);
        }
    }
}