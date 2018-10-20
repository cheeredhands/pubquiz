using System;
using System.Collections.Generic;

namespace Pubquiz.Domain.ViewModels
{
    public class TeamLobbyViewModel
    {
        public Guid TeamId { get; set; }
        public TeamViewModel Team { get; set; }
        public List<TeamViewModel> OtherTeamsInGame { get; set; }
    }
}