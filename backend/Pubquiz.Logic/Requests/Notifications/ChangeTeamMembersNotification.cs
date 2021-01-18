using MediatR;
using Pubquiz.Domain.Models;

namespace Pubquiz.Logic.Requests.Notifications
{
    /// <summary>
    /// Notification to change the <see cref="Team"/> members.
    /// </summary>
    public class ChangeTeamMembersNotification : INotification
    {
        public string TeamId { get; set; }
        public string TeamMembers { get; set; }
    }
}