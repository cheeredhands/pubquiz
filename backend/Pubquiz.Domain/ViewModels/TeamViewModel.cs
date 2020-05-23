using Pubquiz.Domain.Models;

namespace Pubquiz.Domain.ViewModels
{
    public class TeamViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MemberNames { get; set; }
        public bool IsLoggedIn { get; set; }

        public TeamViewModel(Team team)
        {
            Id = team.Id;
            Name = team.Name;
            MemberNames = team.MemberNames;
            IsLoggedIn = team.IsLoggedIn;
        }
    }
}