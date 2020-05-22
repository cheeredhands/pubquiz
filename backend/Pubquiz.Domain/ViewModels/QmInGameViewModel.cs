using Pubquiz.Domain.Models;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Pubquiz.Domain.ViewModels
{
    public class QmInGameViewModel
    {
        public Game Game { get; set; }
        public QuizItem CurrentQuizItem { get; set; }
        public QmTeamFeedViewModel QmTeamFeed { get; set; }
        public QmTeamRankingViewModel QmTeamRanking { get; set; }
    }
}