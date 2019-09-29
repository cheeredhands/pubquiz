using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Rebus.Bus;
using GameStateChanged = Pubquiz.Logic.Messages.GameStateChanged;

namespace Pubquiz.Logic.Requests
{
    /// <summary>
    /// Notification to set the <see cref="GameState">state</see> of a specific <see cref="Game"/>.
    /// </summary>
    [ValidateEntity(EntityType = typeof(User),IdPropertyName = "ActorId")]
    [ValidateEntity(EntityType = typeof(Game), IdPropertyName = "GameId")]
    public class SetGameStateNotification : Notification
    {
        public string ActorId { get; set; }
        public string GameId { get; set; }
        public GameState NewGameState { get; set; }

        public SetGameStateNotification(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork, bus)
        {
        }

        protected override async Task DoExecute()
        {
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(GameId);
            
            var user = await UnitOfWork.GetCollection<User>().GetAsync(ActorId);

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