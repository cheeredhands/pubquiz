using System;
using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Rebus.Bus;

namespace Pubquiz.Logic.Requests
{
    public class SetGameStateNotification : Notification
    {
        public Guid GameId { get; set; }
        public GameState NewGameState { get; set; }

        public SetGameStateNotification(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork, bus)
        {
        }

        protected override async Task DoExecute()
        {
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(GameId);
            if (game == null)
            {
                throw new DomainException(ErrorCodes.InvalidGameId, "Invalid game id.", false);
            }

            game.SetState(NewGameState);

            await gameCollection.UpdateAsync(game);
        }
    }
}