using MediatR;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;

namespace Pubquiz.Logic.Requests.Notifications
{
    /// <summary>
    /// Command to change the <see cref="Team"/> name.
    /// </summary>
    [ValidateEntity(EntityType = typeof(Team), IdPropertyName = "TeamId")]
    public class ChangeTeamNameCommand : IRequest
    {
        public string TeamId;
        public string NewName;
    }
}