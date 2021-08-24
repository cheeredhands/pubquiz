using Pubquiz.Domain.Models;

namespace Pubquiz.Domain.ViewModels
{
    public class QmGameViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string QuizId { get; set; }
        public string QuizTitle { get; set; }
        public string InviteCode { get; set; }
        public GameState GameState { get; set; }
    }
}