using MediatR;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;

namespace Pubquiz.Logic.Requests.Notifications
{
    [ValidateEntity(EntityType = typeof(Team), IdPropertyName = "TeamId")]
    public class LogoutTeamCommand : IRequest
    {
        public string TeamId { get; set; }
    }
}