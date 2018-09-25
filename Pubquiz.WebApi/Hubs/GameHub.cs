using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Pubquiz.Domain.Models;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace Pubquiz.WebApi.Hubs
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
        public async Task TeamRegistered(Team team)
        {
            if (team == null) throw new ArgumentException(nameof(team));

            // add to the teams group
            var teamGroupId = GetTeamsGroupId(team.GameId);
            await Groups.AddToGroupAsync(Context.ConnectionId, teamGroupId);
            
            // notify quiz master 
            var quizMasterGroupId = GetQuizMasterGroupId(team.GameId);
            await Clients.Group(quizMasterGroupId).TeamRegisteredAsync(team);

            // notify other teams
            await Clients.Group(teamGroupId).TeamRegisteredAsync(team);
        }

        /// <summary>
        /// Called when the quiz master registers for the <param name="game">.</param>.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task QuizMasterRegistered(Game game, User user)
        {
            if (game == null) throw new ArgumentException(nameof(game));
            if (user == null) throw new ArgumentException(nameof(user));
            
            // add this connection to the quiz master group
            var quizmasterGroupId = GetQuizMasterGroupId(game);
            await Groups.AddToGroupAsync(Context.ConnectionId, quizmasterGroupId);
        }
        
        public async Task TeamNameUpdated(Team team)
        {
            if (team == null) throw new ArgumentException(nameof(team));

            // notify quiz master 
            var quizMasterGroupId = GetQuizMasterGroupId(team.GameId);
            await Clients.Group(quizMasterGroupId).TeamNameUpdatedAsync(team);

            // notify other teams
            var teamGroupId = GetTeamsGroupId(team.GameId);
            await Clients.Group(teamGroupId).TeamNameUpdatedAsync(team);
        }

        public async Task TeamIsTyping(Team team, Question question, bool isTyping)
        {
            if (team == null) throw new ArgumentException(nameof(team));
            if (question == null) throw new ArgumentException(nameof(question));

            // notify quiz master 
            var quizMasterGroupId = GetQuizMasterGroupId(team.GameId);
            await Clients.Group(quizMasterGroupId).TeamIsTypingAsync(team, question, isTyping);
        }

        public async Task AnswerRequiresReview(Guid gameId, Answer answer)
        {
            if (answer == null) throw new ArgumentException(nameof(answer));

            // TODO: include team?
            
            // notify quiz master 
            var quizMasterGroupId = GetQuizMasterGroupId(gameId);
            await Clients.Group(quizMasterGroupId).AnswerRequiresReviewAsync(answer);
        }

        public async Task GameStateChanged(Game game)
        {
            if (game == null) throw new ArgumentException(nameof(game));

            // notify the teams
            var teamGroupId = GetTeamsGroupId(game);
            await Clients.Group(teamGroupId).GameStateChangedAsync(game);
        }

        public async Task CurrentQuestionIndexChanged(Game game)
        {
            if (game == null) throw new ArgumentException(nameof(game));

            // notify the teams
            var teamGroupId = GetTeamsGroupId(game);
            await Clients.Group(teamGroupId).CurrentQuestionIndexChangedAsync(game);
        }

        public async Task ScoresReleased(Game game)
        {
            if (game == null) throw new ArgumentException(nameof(game));
            
            // notify the teams
            var teamGroupId = GetTeamsGroupId(game);
            await Clients.Group(teamGroupId).ScoresReleasedAsync(game);
        }

        private static string GetTeamsGroupId(Game game)
        {
            return GetTeamsGroupId(game.Id);
        }

        private static string GetTeamsGroupId(Guid gameId)
        {
            // TODO: add as method to Team class?
            return $"teams-{gameId}";
        }

        private static string GetQuizMasterGroupId(Game game)
        {
            return GetQuizMasterGroupId(game.Id);
        }

        private static string GetQuizMasterGroupId(Guid gameId)
        {
            // TODO: add as method to Game class?
            return $"quizmaster-{gameId}";
        }
    }
}