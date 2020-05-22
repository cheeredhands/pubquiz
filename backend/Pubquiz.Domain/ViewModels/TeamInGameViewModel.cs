using Pubquiz.Domain.Models;
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Pubquiz.Domain.ViewModels
{
    public class TeamInGameViewModel
    {
        public Game Game { get; set; }
        public QuizItemViewModel QuizItemViewModel { get; set; }
    }
}