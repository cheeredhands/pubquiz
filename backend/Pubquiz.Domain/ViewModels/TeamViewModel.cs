using Pubquiz.Domain.Models;

namespace Pubquiz.Domain.ViewModels
{
    public class TeamViewModel
    {
        public string TeamId { get; set; }
        public string CurrentGameId { get; set; }
        public string TeamName { get; set; }
        public string MemberNames { get; set; }
        public bool IsLoggedIn { get; set; }

        public TeamViewModel(Team team)
        {
            TeamId = team.Id;
            CurrentGameId = team.CurrentGameId;
            TeamName = team.Name;
            MemberNames = team.MemberNames;
            IsLoggedIn = team.IsLoggedIn;
        }
    }
}