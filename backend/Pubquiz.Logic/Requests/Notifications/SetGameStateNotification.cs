using MediatR;
using Pubquiz.Domain.Models;

namespace Pubquiz.Logic.Requests.Notifications
{
    /// <summary>
    /// Notification to set the <see cref="GameState">state</see> of a specific <see cref="Game"/>.
    /// </summary>
    public class SetGameStateNotification : INotification
    {
        public string ActorId { get; set; }
        public string GameId { get; set; }
        public GameState NewGameState { get; set; }
    }

    
}