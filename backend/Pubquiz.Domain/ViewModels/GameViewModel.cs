using Pubquiz.Domain.Models;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Pubquiz.Domain.ViewModels
{
    public class GameViewModel
    {
        public string GameId { get; set; }
        public GameState State { get; set; }
        public string GameTitle { get; set; }

        public int TotalQuestionCount { get; set; }
        public int TotalQuizItemCount { get; set; }
        public int CurrentSectionQuizItemCount { get; set; }
        
        public int CurrentSectionIndex { get; set; }
        public string CurrentSectionId { get; set; }
        public string CurrentQuizItemId { get; set; }
        
        public int CurrentQuizItemIndexInSection { get; set; }
        public int CurrentQuizItemIndexInTotal { get; set; }
        public int CurrentQuestionIndexInTotal { get; set; }
    }
}