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
        IHandleMessages<TeamNameUpdated>
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

        }

        public async Task Handle(InteractionResponseAdded message) 
        {
            // notify clients via hub
            // something like:
            // var sendMessage = new {Code = 100, message.TeamId, message.TeamName, message.QuestionId, message.Response};
        }

        public async Task Handle(TeamMembersChanged message)
        {
            var quizMasterGroupId = Helpers.GetQuizMasterGroupId(message.GameId);

            // notify quiz master 
            await _gameHubContext.Clients.Group(quizMasterGroupId).TeamMembersChanged(message);
        }

        public async Task Handle(ErrorOccurred message)
        {
            
        }

        public async Task Handle(TeamRegistered message)
        {
            var teamGroupId = Helpers.GetTeamsGroupId(message.GameId);
            var quizMasterGroupId = Helpers.GetQuizMasterGroupId(message.GameId);

            // notify quiz master 
            await _gameHubContext.Clients.Group(quizMasterGroupId).TeamRegistered(message);

            // notify other teams
            // todo: pass the connection id in the TeamRegistered message?
            //await clients.AllExcept().OthersInGroup(teamGroupId).TeamRegistered(message);
        }

        public async Task Handle(GameStateChanged message)
        {
            var teamGroupId = Helpers.GetTeamsGroupId(message.GameId);
            var quizMasterGroupId = Helpers.GetQuizMasterGroupId(message.GameId);

            // notify quiz master 
            await _gameHubContext.Clients.Group(quizMasterGroupId).GameStateChanged(message);

            // notify other teams
            // todo: pass the connection id in the TeamRegistered message? or use this message to confirm the change on the caller?
            //await clients.OthersInGroup(teamGroupId).GameStateChanged(message);
        }

        public async Task Handle(TeamNameUpdated message)
        {
            var teamGroupId = Helpers.GetTeamsGroupId(message.GameId);
            var quizMasterGroupId = Helpers.GetQuizMasterGroupId(message.GameId);

            // notify quiz master 
            await _gameHubContext.Clients.Group(quizMasterGroupId).TeamNameUpdated(message);

            // notify other teams
            // todo: pass the connection id in the TeamRegistered message? or use this message to confirm the change on the caller?
            // var clients = _gameHubContext.Clients as IHubCallerClients<IGameHub>;
            // await clients.OthersInGroup(teamGroupId).TeamNameUpdated(message);
        }
    }
}