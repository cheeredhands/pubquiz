using System;
using System.Collections.Generic;
using Pubquiz.Domain.Models;

namespace Pubquiz.Domain.ViewModels
{
    public class TeamLobbyViewModel
    {
        public Game Game { get; set; }
        public List<TeamViewModel> OtherTeamsInGame { get; set; }
    }
}