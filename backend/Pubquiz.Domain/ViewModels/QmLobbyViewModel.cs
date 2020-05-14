using System.Collections.Generic;
using Pubquiz.Domain.Models;

namespace Pubquiz.Domain.ViewModels
{
    public class QmLobbyViewModel
    {
        public string UserId { get; set; }
        public GameViewModel CurrentGame { get; set; }
        public List<TeamViewModel> TeamsInGame { get; set; }
        
        
    }
}
