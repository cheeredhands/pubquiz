using System;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Rebus.Bus;

namespace Pubquiz.Logic.Requests
{
    public class DeleteTeamNotification : Notification
    {
        public Guid ActorId { get; set; }
        public Guid TeamId { get; set; }

        public DeleteTeamNotification(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork, bus)
        {
        }

        protected override async Task DoExecute()
        {
            var teamCollection = UnitOfWork.GetCollection<Team>();

            var team = await teamCollection.GetAsync(TeamId);
            if (team == null)
            {
                throw new DomainException(ErrorCodes.InvalidTeamId, "Invalid team id.", false);
            }

            var user = await UnitOfWork.GetCollection<User>().GetAsync(ActorId);
            if (user == null)
            {
                throw new DomainException(ErrorCodes.InvalidUserId, "Invalid actor id.", true);
            }

            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(team.GameId);
            if (user.UserRole != UserRole.Admin)
            {                
                if (game.QuizMasterIds.All(i => i != ActorId))
                {
                    throw new DomainException(ErrorCodes.QuizMasterUnauthorizedForGame,
                        $"Actor with id {ActorId} is not authorized for game '{game.Id}'", true);
                }
            }

            game.TeamIds.Remove(team.Id);
            await gameCollection.UpdateAsync(game);
            await teamCollection.DeleteAsync(TeamId);
            
            
        }
    }
}