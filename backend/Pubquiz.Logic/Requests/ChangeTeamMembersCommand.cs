using System;
using System.Text.RegularExpressions;
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
    public class ChangeTeamMembersCommand : Command<string>
    {
        public string TeamId { get; set; }
        public string TeamMembers { get; set; }

        public ChangeTeamMembersCommand(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork, bus)
        {
        }

        protected override async Task<string> DoExecute()
        {
            if (TeamMembers.Length > ValidationValues.MaxTeamMembersLength)
            {
                throw new DomainException(ErrorCodes.ValidationError,
                    $"Team members maximum length is {ValidationValues.MaxTeamMembersLength} characters.", true);
            }
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(TeamId);

            TeamMembers = SanitizeTeamMembers(TeamMembers);
            team.MemberNames = TeamMembers;

            await teamCollection.UpdateAsync(team);
            await Bus.Publish(new TeamMembersChanged(team.GameId, TeamId, team.Name, TeamMembers));
            return TeamMembers;
        }

        private string SanitizeTeamMembers(string teamMembers)
        {
            // replace multiple whitespace by just one
            var result = Regex.Replace(teamMembers, @"(\s){2,}", "$1");
            
            // trim whitespace
            return result.Trim();
        }
    }
}