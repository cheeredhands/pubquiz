using System;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Messages;
using Pubquiz.Persistence;
using Rebus.Bus;

namespace Pubquiz.Logic.Requests
{
    public class LogoutTeamNotification : Notification
    {
        public Guid TeamId { get; set; }

        public LogoutTeamNotification(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork, bus)
        {
        }

        protected override async Task DoExecute()
        {
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(TeamId);

            team.IsLoggedIn = false;

            await teamCollection.UpdateAsync(team);
            
            await Bus.Publish(new TeamLoggedOut(team.Id, team.Name, team.GameId));
        }
    }
}