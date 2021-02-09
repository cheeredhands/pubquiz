using MediatR;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;

namespace Pubquiz.Logic.Requests.Commands
{
    /// <summary>
    /// Navigate to the first item in the specified section.
    /// </summary>
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "ActorId")]
    [ValidateEntity(EntityType = typeof(Game), IdPropertyName = "GameId")]
    public class NavigateToSectionCommand : IRequest<string>
    {
        public string GameId { get; set; }
        public string SectionId { get; set; }
        public string ActorId { get; set; }
    }
}