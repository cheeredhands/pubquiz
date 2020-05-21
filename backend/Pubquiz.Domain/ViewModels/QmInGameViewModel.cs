using Pubquiz.Domain.Models;

namespace Pubquiz.Domain.ViewModels
{
    public class QmInGameViewModel
    {
        public string UserId { get; set; }
        public Game Game { get; set; }
        public QuizItem CurrentQuizItem { get; set; }
        public QmTeamFeedViewModel QmTeamFeed { get; set; }
        public QmTeamRankingViewModel QmTeamRanking { get; set; }
    }
}