using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Newtonsoft.Json;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Messages;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Rebus.Bus;

namespace Pubquiz.Logic.Requests
{
    /// <summary>
    /// Notification to delete a <see cref="Team"/>.
    /// </summary>
    [ValidateEntity(EntityType = typeof(Team), IdPropertyName = "TeamId")]
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "ActorId")]
    public class DeleteTeamNotification : Notification
    {
        [JsonIgnore]
        public string ActorId { get; set; }
        public string TeamId { get; set; }

        public DeleteTeamNotification(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork, bus)
        {
        }

        protected override async Task DoExecute()
        {
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(TeamId);
            var user = await UnitOfWork.GetCollection<User>().GetAsync(ActorId);

            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(team.CurrentGameId);
            if (user.UserRole != UserRole.Admin)
            {
                if (game.QuizMasterIds.All(i => i != ActorId))
                {
                    throw new DomainException(ResultCode.QuizMasterUnauthorizedForGame,
                        $"Actor with id {ActorId} is not authorized for game '{game.Id}'", true);
                }
            }

            game.TeamIds.Remove(team.Id);
            await gameCollection.UpdateAsync(game);
            await teamCollection.DeleteAsync(TeamId);

            await Bus.Publish(new TeamDeleted(TeamId, game.Id));
        }
    }
}