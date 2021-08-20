using System.Collections.Generic;
using Pubquiz.Domain.Models;

namespace Pubquiz.Domain.ViewModels
{
    public class QmLobbyViewModel
    {
        public string UserId { get; set; }
        public Game Game { get; set; }
        public List<Team> TeamsInGame { get; set; }
        
        public List<QmQuizViewModel> QuizViewModels { get; set; }
        public List<QmGameViewModel> GameViewModels { get; set; }
    }
}
