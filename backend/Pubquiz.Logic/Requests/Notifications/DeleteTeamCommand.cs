using MediatR;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;

namespace Pubquiz.Logic.Requests.Notifications
{
    /// <summary>
    /// Notification to delete a <see cref="Team"/>.
    /// </summary>
    [ValidateEntity(EntityType = typeof(Team), IdPropertyName = "TeamId")]
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "ActorId")]
    public class DeleteTeamCommand : IRequest
    {
        public string ActorId { get; set; }
        public string TeamId { get; set; }
    }
}