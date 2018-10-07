using System;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Rebus.Bus;

namespace Pubquiz.Logic.Requests
{
    public class ChangeTeamNameCommand : Command<Team>
    {
        public Guid TeamId;
        public string NewName;

        public ChangeTeamNameCommand(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork, bus)
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