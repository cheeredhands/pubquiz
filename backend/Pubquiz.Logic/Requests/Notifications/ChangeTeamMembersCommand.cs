using MediatR;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;

namespace Pubquiz.Logic.Requests.Notifications
{
    /// <summary>
    /// Notification to change the <see cref="Team"/> members.
    /// </summary>
    [ValidateEntity(EntityType = typeof(Team), IdPropertyName = "TeamId")]
    public class ChangeTeamMembersCommand : IRequest
    {
        public string TeamId { get; set; }
        public string TeamMembers { get; set; }
    }
}