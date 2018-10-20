using System;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Rebus.Bus;
using TeamMembersChanged = Pubquiz.Logic.Messages.TeamMembersChanged;

namespace Pubquiz.Logic.Requests
{
    /// <summary>
    /// Notification to change the <see cref="Team"/> members.
    /// </summary>
    [ValidateEntity(EntityType = typeof(Team), IdPropertyName = "TeamId")]
    public class ChangeTeamMembersNotification : Notification
    {
        public Guid TeamId { get; set; }
        public string TeamMembers { get; set; }

        public ChangeTeamMembersNotification(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork, bus)
        {
        }

        protected override async Task DoExecute()
        {
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(TeamId);
            team.MemberNames = TeamMembers;

            await teamCollection.UpdateAsync(team);
            await Bus.Publish(new TeamMembersChanged(team.GameId, TeamId, team.Name, TeamMembers));
        }
    }
}