using MediatR;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;

namespace Pubquiz.Logic.Requests.Notifications
{
    /// <summary>
    /// Command to select a current <see cref="Game"/>.
    /// </summary>
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "ActorId")]
    [ValidateEntity(EntityType = typeof(Game), IdPropertyName = "GameId")]
    public class SelectGameCommand : IRequest
    {
        public string ActorId { get; set; }
        public string GameId { get; set; }
    }
}