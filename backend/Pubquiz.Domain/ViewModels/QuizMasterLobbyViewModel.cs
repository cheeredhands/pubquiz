using System;
using System.Collections.Generic;
using System.Text;

namespace Pubquiz.Domain.ViewModels
{
    public class QuizMasterLobbyViewModel
    {
        public Guid UserId { get; set; }
        public GameViewModel CurrentGame { get; set; }
        public List<TeamViewModel> TeamsInGame { get; set; }
    }
}
