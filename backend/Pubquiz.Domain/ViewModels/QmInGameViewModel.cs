using Pubquiz.Domain.Models;

namespace Pubquiz.Domain.ViewModels
{
    public class QmInGameViewModel
    {
        public string UserId { get; set; }
        public Game Game { get; set; }
        public QuizItem CurrentQuizItem { get; set; }
        public QmTeamFeedViewModel QmTeamFeedViewModel { get; set; }
        public QmTeamRankingViewModel QmTeamRankingViewModel { get; set; }
    }
}