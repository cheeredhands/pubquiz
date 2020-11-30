using System.Collections.Generic;

namespace Pubquiz.Domain.Models
{
    public class QuizRef
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public List<GameRef> GameRefs { get; set; }

        public QuizRef()
        {
            GameRefs = new List<GameRef>();
        }
    }
}