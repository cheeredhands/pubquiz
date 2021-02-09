using MediatR;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;

namespace Pubquiz.Logic.Requests.Notifications
{
    /// <summary>
    /// Notification to overrule or set the score for an interaction in a quiz item.
    /// </summary>
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "ActorId")]
    [ValidateEntity(EntityType = typeof(Team), IdPropertyName = "TeamId")]
    [ValidateEntity(EntityType = typeof(QuizItem), IdPropertyName = "QuizItemId")]
    public class CorrectInteractionCommand : IRequest
    {
        public string ActorId { get; set; }
        public string TeamId { get; set; }
        public string QuizItemId { get; set; }
        public int InteractionId { get; set; }
        public bool Correct { get; set; }
    }
}