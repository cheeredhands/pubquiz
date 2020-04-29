using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
        [JsonIgnore]
        public string TeamId { get; set; }
        public string TeamMembers { get; set; }

        public ChangeTeamMembersNotification(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork, bus)
        {
        }

        protected override async Task DoExecute()
        {
            if (TeamMembers.Length > ValidationValues.MaxTeamMembersLength)
            {
                throw new DomainException(ResultCode.ValidationError,
                    $"Team members maximum length is {ValidationValues.MaxTeamMembersLength} characters.", true);
            }
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(TeamId);

            TeamMembers = SanitizeTeamMembers(TeamMembers);
            team.MemberNames = TeamMembers;

            await teamCollection.UpdateAsync(team);
            await Bus.Publish(new TeamMembersChanged(team.CurrentGameId, TeamId, team.Name, TeamMembers));
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