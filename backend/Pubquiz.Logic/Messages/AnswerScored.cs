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
        public int TotalTeamScore { get; set; }
        public Dictionary<string, int> ScorePerQuizSection { get; set; }
        public Answer Answer { get; set; }
    }
}