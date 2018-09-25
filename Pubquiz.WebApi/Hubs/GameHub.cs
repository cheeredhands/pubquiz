using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Pubquiz.Domain.Models;

namespace Pubquiz.WebApi.Hubs
{
    /// <summary>
    /// The GameHub is responsible for communicating relevant changes in a game.
    /// To be able to manage the connection properly without the risk of sending messages
    /// to the wrong clients, we create groups.
    /// On opening a <see cref="Game"/>, the following groups are created:
    /// - a group for the quizmaster
    /// - a group for the teams
    /// </summary>
    public class GameHub: Hub
    {
        /// <summary>
        /// Called when a team registered for a game. Notifies other teams and the quizmaster
        /// within the current game.
        /// </summary>
        /// <param name="team">The team </param>
        /// <returns></returns>
        public async Task TeamRegistered(Team team)
        {
            if (team == null) return;
            
            // notify others in the group identified by the gameId
            var gameId = team.GameId;
            Clients.OthersInGroup(gameId.ToString()).SendAsync("teamRegistered", team);
        }

        /// <summary>
        /// Called when a team updates its name.
        /// </summary>
        /// <param name="team">The team with the new name.</param>
        /// <returns></returns>
        public async Task TeamNameUpdated(Team team)
        {
            if (team == null) return;

            // notify others in the group identified by the gameId
            var gameId = team.GameId;
            Clients.OthersInGroup(gameId.ToString()).SendAsync("teamNameUpdated", team);
        }

        /// <summary>
        /// Called when a team is typing an answer.
        /// </summary>
        /// <param name="team">The current team.</param>
        /// <param name="question">The current question the team is typing an answer for.</param>
        /// <param name="isTyping">true, if the team is typing an answer, otherwise false.</param>
        /// <returns></returns>
        public async Task TeamIsTyping(Team team, Question question, bool isTyping)
        {
            if (team == null) return;

            // notify others in the group identified by the gameId
            var gameId = team.GameId;
            Clients.OthersInGroup(gameId.ToString()).SendAsync("teamIsTyping", team, question, isTyping);
        }

        /// <summary>
        /// Called when an answer can no be scored automatically and the quizmaster must
        /// score manually.
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        public async Task AnswerRequiresReview(Answer answer)
        {
            if (answer == null) return;
            
            // How to make sure only the quizmaster can connect? Via groups?
        }

        /// <summary>
        /// Called when the quizmaster changes the state of the game.
        /// </summary>
        /// <param name="game">The game with the updated <see cref="GameState"/>.</param>
        /// <returns></returns>
        public async Task GameStateChanged(Game game)
        {
            if (game == null) return;
            
            // ...
        }

        /// <summary>
        /// Called when the quizmaster navigates to the another question.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public async Task CurrentQuestionIndexChanged(Game game)
        {
            if (game == null) return;
            
            // ...
        }

        /// <summary>
        /// Called when the quizmaster releases the scores for the game.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public async Task ScoresReleased(Game game)
        {
            
        }
    }
}