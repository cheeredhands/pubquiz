using MediatR;
using Pubquiz.Domain.Models;

namespace Pubquiz.Logic.Requests.Queries
{
    public class TeamQuery : IRequest<Team>
    {
        public string TeamId { get; set; }
    }
}