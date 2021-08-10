using System.Collections.Generic;
using MediatR;
using Pubquiz.Domain.ViewModels;

namespace Pubquiz.Logic.Requests.Queries
{
    public class QmGameViewModelsQuery : IRequest<List<QmGameViewModel>>
    {
        public List<string> GameIds { get; set; }
    }
}