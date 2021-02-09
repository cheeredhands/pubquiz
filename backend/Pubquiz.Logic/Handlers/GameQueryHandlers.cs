using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Requests.Queries;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Handlers
{
    public class GameQueryHandlers : Handler, IRequestHandler<GameQuery, Game>
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
    }
}