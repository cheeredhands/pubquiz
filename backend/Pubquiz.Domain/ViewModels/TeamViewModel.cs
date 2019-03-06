using System;
using Pubquiz.Domain.Models;

namespace Pubquiz.Domain.ViewModels
{
    public class TeamViewModel
    {
        public Guid TeamId { get; set; }
        public string TeamName { get; set; }
        public string MemberNames { get; set; }
        public bool IsLoggedIn { get; set; }

        public TeamViewModel(Team team)
        {
            TeamId = team.Id;
            TeamName = team.Name;
            MemberNames = team.MemberNames;
            IsLoggedIn = team.IsLoggedIn;
        }
    }
}