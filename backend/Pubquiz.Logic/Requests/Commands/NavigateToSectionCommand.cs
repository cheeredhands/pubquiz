using MediatR;

namespace Pubquiz.Logic.Requests.Commands
{
    /// <summary>
    /// Navigate to the first item in the specified section.
    /// </summary>
    public class NavigateToSectionCommand : IRequest<string>
    {
        public string GameId { get; set; }
        public string SectionId { get; set; }
        public string ActorId { get; set; }
    }
}