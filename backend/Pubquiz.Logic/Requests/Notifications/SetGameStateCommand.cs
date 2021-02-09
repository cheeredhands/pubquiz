using MediatR;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;

namespace Pubquiz.Logic.Requests.Notifications
{
    /// <summary>
    /// Notification to set the <see cref="GameState">state</see> of a specific <see cref="Game"/>.
    /// </summary>
    [ValidateEntity(EntityType = typeof(User),IdPropertyName = "ActorId")]
    [ValidateEntity(EntityType = typeof(Game), IdPropertyName = "GameId")]
    public class SetGameStateCommand : IRequest
    {
        public string ActorId { get; set; }
        public string GameId { get; set; }
        public GameState NewGameState { get; set; }
    }

    
}