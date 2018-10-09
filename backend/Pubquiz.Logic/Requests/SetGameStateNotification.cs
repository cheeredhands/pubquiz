using System;
using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Messages;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Rebus.Bus;

namespace Pubquiz.Logic.Requests
{
    public class SetGameStateNotification : Notification
    {
        public Guid ActorId { get; set; }
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

            var user = await UnitOfWork.GetCollection<User>().GetAsync(ActorId);
            if (user == null)
            {
                throw new DomainException(ErrorCodes.InvalidUserId, "Invalid actor id.", true);
            }

            if (user.UserRole != UserRole.Admin)
            {
                if (game.QuizMasterIds.All(i => i != ActorId))
                {
                    throw new DomainException(ErrorCodes.QuizMasterUnauthorizedForGame,
                        $"Actor with id {ActorId} is not authorized for game '{game.Id}'", true);
                }
            }

            var oldGameState = game.State;

            game.SetState(NewGameState);

            await gameCollection.UpdateAsync(game);

            await Bus.Publish(new GameStateChanged(GameId, oldGameState, NewGameState));
        }
    }

    
}