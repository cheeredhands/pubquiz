using MediatR;

namespace Pubquiz.Logic.Requests.Notifications
{
    /// <summary>
    /// Notification to set the game to the Reviewing state and navigate to the first item in the specified section.
    /// </summary>
    public class SetReviewNotification : INotification
    {
        public string ActorId { get; set; }
        public string GameId { get; set; }
        public string SectionId { get; set; }
    }
}