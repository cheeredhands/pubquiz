using MediatR;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.ViewModels;
using Pubquiz.Logic.Tools;

namespace Pubquiz.Logic.Requests.Queries
{
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "ActorId")]
    public class QmInGameViewModelQuery : IRequest<QmInGameViewModel>
    {
        public string ActorId { get; set; }
    }
}