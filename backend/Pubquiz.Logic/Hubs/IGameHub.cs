using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Pubquiz.Logic.Messages;

namespace Pubquiz.Logic.Hubs
{
    public interface IGameHub : IClientProxy
    {
        /// <summary>
        /// Called when a team registered for a game. Notifies other teams and the quizmaster
        /// within the current game.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task TeamRegistered(TeamRegistered message);

        /// <summary>
        /// Called when a team logs out (leaves the game). Notifies other teams and the quizmaster
        /// within the current game.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task TeamLoggedOut(TeamLoggedOut message);

        /// <summary>
        /// Called when a user logs out (leaves the game). Notifies other teams and the quizmaster
        /// within the current game.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task UserLoggedOut(UserLoggedOut message);

        /// <summary>
        /// Called when a team updates its name.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task TeamNameUpdated(TeamNameUpdated message);

        /// <summary>
        /// Called when a team is deleted.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task TeamDeleted(TeamDeleted message);

        /// <summary>
        /// Called when a team changes its team members.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task TeamMembersChanged(TeamMembersChanged message);


        /// <summary>
        /// Called when the quiz master changes the state of the game.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task GameStateChanged(GameStateChanged message);

        /// <summary>
        /// Called when the quiz master navigates to another quiz item.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task ItemNavigated(ItemNavigated message);

        /// <summary>
        /// Called when a team answers (part of) a quiz item.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task InteractionResponseAdded(InteractionResponseAdded message);
    }
}