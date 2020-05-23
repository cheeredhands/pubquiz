using System.Collections.Generic;
using Pubquiz.Domain.Models;

namespace Pubquiz.Logic.Messages
{
    public class AnswerScored
    {
        public string TeamId { get; set; }
        public string GameId { get; set; }
        public string QuizItemId { get; set; }
        public int InteractionId { get; set; }
        public string Response { get; set; }
        public int QuizItemScore { get; set; }
        public int TotalTeamScore { get; set; }
        public List<InteractionResponse> InteractionResponses { get; set; }
    }
}