using MediatR;
using Pubquiz.Domain.Models;

namespace Pubquiz.Logic.Requests.Queries
{
    public class GameQuery : IRequest<Game>
    {
        public string GameId { get; set; }
    }
}