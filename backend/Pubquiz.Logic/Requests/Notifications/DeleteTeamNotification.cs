using MediatR;
using Pubquiz.Domain.Models;

namespace Pubquiz.Logic.Requests.Notifications
{
    /// <summary>
    /// Notification to delete a <see cref="Team"/>.
    /// </summary>
    public class DeleteTeamNotification : INotification
    {
        public string ActorId { get; set; }
        public string TeamId { get; set; }
    }
}