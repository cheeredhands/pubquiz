using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Messages;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Hubs
{
    /// <summary>
    /// The GameHub is responsible for communicating relevant changes in a game.
    /// To be able to manage the connection properly without the risk of sending messages
    /// to the wrong clients, we create groups.
    /// When a quiz master registers through <see cref="QuizMasterRegistered"/> two groups
    /// are created.
    /// - a group for the quiz master
    /// - a group for the teams
    /// To isolate group names within a game, the id of the game is used as postfix. So
    /// there will be a group called quizmaster-<gameId> and a group for the teams
    /// called teams-<gameId>.
    /// Whenever a connection is lost, group membership MUST be rebuild.
    /// </summary>
    public class GameHub : Hub<IGameHub>
    {
        public override async Task OnConnectedAsync()
        {
            var userRole = Context.User.GetUserRole();
            var currentGameId = Context.User.GetCurrentGameId();
            switch (userRole)
            {
                case UserRole.Team:
                    var teamGroupId = Helpers.GetTeamsGroupId(currentGameId);
                    await Groups.AddToGroupAsync(Context.ConnectionId, teamGroupId);
                    break;
                case UserRole.Admin:
                    await Groups.AddToGroupAsync(Context.ConnectionId, Helpers.GetAdminGroupId());
                    break;
                case UserRole.QuizMaster:
                    var quizmasterGroupId = Helpers.GetQuizMasterGroupId(currentGameId);
                    await Groups.AddToGroupAsync(Context.ConnectionId, quizmasterGroupId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userRole = Context.User.GetUserRole();
            var currentGameId = Context.User.GetCurrentGameId();
            switch (userRole)
            {
                case UserRole.Team:
                    var teamGroupId = Helpers.GetTeamsGroupId(currentGameId);
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, teamGroupId);
                    break;
                case UserRole.Admin:
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, Helpers.GetAdminGroupId());
                    break;
                case UserRole.QuizMaster:
                    var quizmasterGroupId = Helpers.GetQuizMasterGroupId(currentGameId);
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, quizmasterGroupId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await base.OnDisconnectedAsync(exception);
        }

//        public async Task NotifyTeamRegistered(TeamRegistered message)
//        {
//            if (message == null) throw new ArgumentException(nameof(message));
//
//            var teamGroupId = GetTeamsGroupId(message.GameId);
//            var quizMasterGroupId = GetQuizMasterGroupId(message.GameId);
//
//            // notify quiz master 
//            await Clients.Group(quizMasterGroupId).TeamRegistered(message);
//
//            // notify other teams
//            await Clients.OthersInGroup(teamGroupId).TeamRegistered(message);
//        }

        //        public async Task TeamRegisteredAsync(TeamRegistered message)
        //        {
        //            if (message == null) throw new ArgumentException(nameof(message));
        //
        //            var teamGroupId = GetTeamsGroupId(message.GameId);
        //            var quizMasterGroupId = GetQuizMasterGroupId(message.GameId);
        //
        //            // notify quiz master 
        //            await Clients.Group(quizMasterGroupId).TeamRegisteredAsync(message);
        //
        //            // notify other teams
        //            await Clients.OthersInGroup(teamGroupId).TeamRegisteredAsync(message);
        //        }

        //        public async Task TeamMembersChangedAsync(TeamMembersChanged message)
        //        {
        //            if (message == null) throw new ArgumentException(nameof(message));
        //
        //            // notify quiz master 
        //            var quizMasterGroupId = GetQuizMasterGroupId(message.GameId);
        //            await Clients.Group(quizMasterGroupId).TeamMembersChangedAsync(message);
        //        }

        //        public async Task TeamNameUpdatedAsync(TeamNameUpdated message)
        //        {
        //            if (message == null) throw new ArgumentException(nameof(message));
        //
        //            // notify quiz master 
        //            var quizMasterGroupId = GetQuizMasterGroupId(message.GameId);
        //            await Clients.Group(quizMasterGroupId).TeamNameUpdatedAsync(message);
        //
        //            // notify other teams
        //            var teamGroupId = GetTeamsGroupId(message.GameId);
        //            await Clients.Group(teamGroupId).TeamNameUpdatedAsync(message);
        //        }
        //
        //        public async Task TeamIsTypingAsync(Team team, Question question, bool isTyping)
        //        {
        //            if (team == null) throw new ArgumentException(nameof(team));
        //            if (question == null) throw new ArgumentException(nameof(question));
        //
        //            // notify quiz master 
        //            var quizMasterGroupId = GetQuizMasterGroupId(team.GameId);
        //            await Clients.Group(quizMasterGroupId).TeamIsTypingAsync(team, question, isTyping);
        //        }
        //
        //        public async Task AnswerRequiresReviewAsync(Guid gameId, Answer answer)
        //        {
        //            if (answer == null) throw new ArgumentException(nameof(answer));
        //
        //            // TODO: include team?
        //
        //            // notify quiz master 
        //            var quizMasterGroupId = GetQuizMasterGroupId(gameId);
        //            await Clients.Group(quizMasterGroupId).AnswerRequiresReviewAsync(answer);
        //        }
        //
        //        public async Task GameStateChangedAsync(GameStateChanged message)
        //        {
        //            if (message == null) throw new ArgumentException(nameof(message));
        //
        //            // notify the teams
        //            var teamGroupId = GetTeamsGroupId(message.GameId);
        //            await Clients.Group(teamGroupId).GameStateChangedAsync(message);
        //        }
        //
        //        public async Task CurrentQuestionIndexChangedAsync(Game game)
        //        {
        //            if (game == null) throw new ArgumentException(nameof(game));
        //
        //            // notify the teams
        //            var teamGroupId = GetTeamsGroupId(game.Id);
        //            await Clients.Group(teamGroupId).CurrentQuestionIndexChangedAsync(game);
        //        }
        //
        //        public async Task ScoresReleasedAsync(Game game)
        //        {
        //            if (game == null) throw new ArgumentException(nameof(game));
        //
        //            // notify the teams
        //            var teamGroupId = GetTeamsGroupId(game.Id);
        //            await Clients.Group(teamGroupId).ScoresReleasedAsync(game);
        //        }
        //
        //        public async Task QuizmasterRegisteredAsync(Game game)
        //        {
        //            var quizMasterGroupId = GetQuizMasterGroupId(game.Id);
        //            await Groups.AddToGroupAsync(Context.ConnectionId, quizMasterGroupId);
        //        }
    }
}