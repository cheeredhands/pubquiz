using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Requests.Commands;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Rebus.Bus;

namespace Pubquiz.Logic.Requests.Notifications
{
    /// <summary>
    /// Notification to set the game to the Reviewing state and navigate to the first item in the specified section.
    /// </summary>
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "ActorId")]
    [ValidateEntity(EntityType = typeof(Game), IdPropertyName = "GameId")]
    public class SetReviewNotification : Notification
    {
        public string ActorId { get; set; }
        public string GameId { get; set; }
        public string SectionId { get; set; }

        public SetReviewNotification(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork, bus)
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
                    throw new DomainException(ResultCode.QuizMasterUnauthorizedForGame,
                        $"Actor with id {ActorId} is not authorized for game '{game.Id}'", true);
                }
            }

            var command = new NavigateToSectionCommand(UnitOfWork, Bus)
            {
                ActorId = ActorId, GameId = GameId, SectionId = SectionId
            };
            await command.Execute();

            var notification = new SetGameStateNotification(UnitOfWork, Bus)
            {
                ActorId = ActorId, GameId = GameId, NewGameState = GameState.Reviewing
            };
            await notification.Execute();
        }
    }
}