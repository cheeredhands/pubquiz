using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Rebus.Bus;

namespace Pubquiz.Logic.Requests
{
    /// <summary>
    /// Command to select a current <see cref="Game"/>.
    /// </summary>
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "ActorId")]
    [ValidateEntity(EntityType = typeof(Game), IdPropertyName = "GameId")]
    public class SelectGameCommand : Command<User>
    {
        public string ActorId { get; set; }
        public string GameId { get; set; }

        public SelectGameCommand(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork, bus)
        {
        }

        protected override async Task<User> DoExecute()
        {
            var userCollection = UnitOfWork.GetCollection<User>();
            var user = await userCollection.GetAsync(ActorId);
            if (user.UserRole != UserRole.QuizMaster)
            {
                throw new DomainException(ErrorCodes.UnauthorizedRole, "You can't do that with this role.", true);
            }

            if (!user.GameIds.Contains(GameId))
            {
                throw new DomainException(ErrorCodes.QuizMasterUnauthorizedForGame,
                    $"Actor with id {ActorId} is not authorized for game '{GameId}'", true);
            }

            user.CurrentGameId = GameId;
            await userCollection.UpdateAsync(user);
            return user;
        }
    }
}