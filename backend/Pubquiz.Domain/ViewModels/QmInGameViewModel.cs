using System.Collections.Generic;

namespace Pubquiz.Domain.ViewModels
{
    public class QmInGameViewModel
    {
        public string UserId { get; set; }
        public GameViewModel Game { get; set; }
        public QmQuizViewModel Quiz { get; set; }
        public List<TeamViewModel> TeamsInGame { get; set; }
    }
}