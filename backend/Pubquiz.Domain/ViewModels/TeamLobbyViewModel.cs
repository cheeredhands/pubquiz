using System;
using System.Collections.Generic;

namespace Pubquiz.Domain.ViewModels
{
    public class TeamLobbyViewModel
    {
        public string TeamId { get; set; }
        public TeamViewModel Team { get; set; }
        public List<TeamViewModel> OtherTeamsInGame { get; set; }
    }
}