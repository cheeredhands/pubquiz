using System;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Messages;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Rebus.Bus;

namespace Pubquiz.Logic.Requests.Notifications
{
    /// <summary>
    /// Command to change the <see cref="Team"/> name.
    /// </summary>
    [ValidateEntity(EntityType = typeof(Team), IdPropertyName = "TeamId")]
    public class ChangeTeamNameNotification : Notification
    {
        public string TeamId;
        public string NewName;

        public ChangeTeamNameNotification(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork, bus)
        {
        }

        protected override async Task DoExecute()
        {
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(TeamId);

            // check if team name is taken, otherwise throw DomainException
            var isTeamNameTaken = await teamCollection.AnyAsync(t =>
                String.Equals(t.Name, NewName, StringComparison.CurrentCultureIgnoreCase) &&
                t.CurrentGameId == team.CurrentGameId);
            if (isTeamNameTaken)
            {
                throw new DomainException(ResultCode.TeamNameIsTaken, "Team name is taken.", true);
            }

            // set new name
            var oldTeamName = team.Name;
            team.Name = NewName.Trim();
            team.UserName = NewName.Trim();
            await teamCollection.UpdateAsync(team);
            await Bus.Publish(new TeamNameUpdated(TeamId, team.CurrentGameId, oldTeamName, NewName));
        }
    }
}