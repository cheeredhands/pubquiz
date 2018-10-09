using System;
using System.Collections.Generic;

namespace Pubquiz.Domain.ViewModels
{
    public class TeamLobbyViewModel
    {
        public Guid TeamId { get; set; }
        public string TeamName { get; set; }
        public string TeamMembers { get; set; }
        public List<string> OtherTeamsInGame { get; set; }
    }
}