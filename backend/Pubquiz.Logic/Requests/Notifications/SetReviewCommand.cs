using MediatR;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;

namespace Pubquiz.Logic.Requests.Notifications
{
    /// <summary>
    /// Notification to set the game to the Reviewing state and navigate to the first item in the specified section.
    /// </summary>
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "ActorId")]
    [ValidateEntity(EntityType = typeof(Game), IdPropertyName = "GameId")]
    public class SetReviewCommand : IRequest
    {
        public string ActorId { get; set; }
        public string GameId { get; set; }
        public string SectionId { get; set; }
    }
}