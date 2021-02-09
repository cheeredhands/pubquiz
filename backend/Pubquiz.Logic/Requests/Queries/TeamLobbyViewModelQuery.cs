using MediatR;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.ViewModels;
using Pubquiz.Logic.Tools;

namespace Pubquiz.Logic.Requests.Queries
{
    /// <summary>
    /// Query to get the <see cref="TeamLobbyViewModel"/> for a specific <see cref="Team"/>.
    /// </summary>
    [ValidateEntity(EntityType = typeof(Team), IdPropertyName = "TeamId")]
    public class TeamLobbyViewModelQuery : IRequest<TeamLobbyViewModel>
    {
        public string TeamId { get; set; }
    }
}