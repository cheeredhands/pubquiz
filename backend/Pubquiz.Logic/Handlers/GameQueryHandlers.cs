using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.ViewModels;
using Pubquiz.Logic.Requests.Queries;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Handlers
{
    public class GameQueryHandlers : Handler, IRequestHandler<GameQuery, Game>,
        IRequestHandler<QmGameViewModelsQuery, List<QmGameViewModel>>
    {
        public GameQueryHandlers(IUnitOfWork unitOfWork, IMediator mediator, ILoggerFactory loggerFactory) : base(
            unitOfWork, mediator, loggerFactory)
        {
        }

        public async Task<Game> Handle(GameQuery request, CancellationToken cancellationToken)
        {
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(request.GameId);
            return game;
        }

        public async Task<List<QmGameViewModel>> Handle(QmGameViewModelsQuery request,
            CancellationToken cancellationToken)
        {
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var games = await gameCollection.GetAsync(request.GameIds.ToArray());

            return games.Select(g => g.ToQmGameViewModel()).ToList();
        }
    }
}