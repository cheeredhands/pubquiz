using System;
using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Domain.Requests
{
    public class ChangeTeamMembersNotification : Notification
    {
        public Guid TeamId { get; set; }
        public string TeamMembers { get; set; }

        public ChangeTeamMembersNotification(IUnitOfWork unitOfWork) : base(unitOfWork)
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
        }
    }

    public class ChangeTeamNameCommand : Command<Team>
    {
        public Guid TeamId;
        public string NewName;

        public ChangeTeamNameCommand(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override async Task<Team> DoExecute()
        {
            // check team exists
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(TeamId);
            if (team == null)
            {
                throw new DomainException(3, "Invalid team id.", false);
            }

            // check if team name is taken, otherwise throw DomainException
            var isTeamNameTaken = await teamCollection.AnyAsync(t => t.Name == NewName && t.GameId == team.GameId);
            if (isTeamNameTaken)
            {
                throw new DomainException(2, "Team name is taken.", true);
            }

            // set new name
            team.Name = NewName;
            team.UserName = NewName.ReplaceSpaces();
            await teamCollection.UpdateAsync(team);
            return team;
        }
    }
}