using MediatR;
using Pubquiz.Domain.ViewModels;

namespace Pubquiz.Logic.Requests.Queries
{
    //[ValidateEntity(EntityType = typeof(User), IdPropertyName = "ActorId")]
    public class TeamInGameViewModelQuery: IRequest<TeamInGameViewModel>
    {
        public string ActorId { get; set; }
    }
}