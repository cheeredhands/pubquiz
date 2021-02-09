using MediatR;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;

namespace Pubquiz.Logic.Requests.Commands
{
    [ValidateEntity(EntityType = typeof(Game), IdPropertyName = "GameId")]
    public class NavigateToItemByOffsetCommand : IRequest<string>
    {
        public int Offset { get; set; }
        public string GameId { get; set; }
        public string ActorId { get; set; }
    }
}