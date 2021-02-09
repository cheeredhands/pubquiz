using System.Collections.Generic;
using MediatR;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;

namespace Pubquiz.Logic.Requests.Notifications
{
    /// <summary>
    /// Notification to submit an interaction response for a certain <see cref="Interaction"/> in a <see cref="QuizItem"/>
    /// </summary>
    [ValidateEntity(EntityType = typeof(Team), IdPropertyName = "TeamId")]
    [ValidateEntity(EntityType = typeof(QuizItem), IdPropertyName = "QuizItemId")]
    public class SubmitInteractionResponseCommand : IRequest
    {
        public string TeamId { get; set; }
        public string QuizItemId { get; set; }
        public int InteractionId { get; set; }
        public List<int> ChoiceOptionIds { get; set; }
        public string Response { get; set; }
    }
}