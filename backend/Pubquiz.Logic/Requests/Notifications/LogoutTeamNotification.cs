using System.Threading.Tasks;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Messages;
using Pubquiz.Persistence;
using Rebus.Bus;

namespace Pubquiz.Logic.Requests.Notifications
{
    public class LogoutTeamNotification : Notification
    {
        public string TeamId { get; set; }

        public LogoutTeamNotification(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork, bus)
        {
        }

        protected override async Task DoExecute()
        {
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(TeamId);
            if (team==null)
            {
                // Unknown team (probably old session with inmemory db), proceed with logout.
                return;
            }
            team.IsLoggedIn = false;

            await teamCollection.UpdateAsync(team);
            
            await Bus.Publish(new TeamLoggedOut(team.Id, team.Name, team.CurrentGameId));
        }
    }
}