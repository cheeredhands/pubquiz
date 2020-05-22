using System;

namespace Pubquiz.Logic.Messages
{
    public class InteractionResponseAdded
    {
        public string GameId { get; set; }
        public string TeamId { get; }
        public string QuizSectionId { get; }
        public string QuizItemId { get; }
        public string Response { get; }

        public InteractionResponseAdded(string gameId, string teamId, string quizSectionId, string quizItemId,
            string response)
        {
            GameId = gameId;
            TeamId = teamId;
            QuizSectionId = quizSectionId;
            QuizItemId = quizItemId;
            Response = response;
        }
    }
}