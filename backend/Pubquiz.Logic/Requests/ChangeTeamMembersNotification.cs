using System;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Messages;
using Pubquiz.Persistence;
using Rebus.Bus;

namespace Pubquiz.Logic.Requests
{
    public class ChangeTeamMembersNotification : Notification
    {
        public Guid TeamId { get; set; }
        public string TeamMembers { get; set; }

        public ChangeTeamMembersNotification(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork, bus)
        {
        }

        protected override async Task DoExecute()
        {
            // check team exists
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(TeamId);
            if (team == null)
            {
                throw new DomainException(3, "Invalid team id.", false);
            }

            // set new team members
            team.MemberNames = TeamMembers;

            await teamCollection.UpdateAsync(team);
            await Bus.Publish(new TeamMembersChanged());
        }
    }
}