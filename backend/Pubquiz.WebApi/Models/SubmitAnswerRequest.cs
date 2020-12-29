using System.Collections.Generic;

namespace Pubquiz.WebApi.Models
{
    public class SubmitAnswerRequest
    {
        public string QuizItemId { get; set; }
        public int InteractionId { get; set; }
        public string Response { get; set; }
        public List<int> ChoiceOptionIds { get; set; }
    }
}