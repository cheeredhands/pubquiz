using System;

namespace Pubquiz.Logic.Messages
{
    public class InteractionResponseAdded
    {
        public string TeamId { get; }
        public string TeamName { get; }
        public string QuizSectionId { get; }
        public string QuestionId { get; }
        public string Response { get; }

        public InteractionResponseAdded(string teamId, string teamName, string quizSectionId, string questionId, string response)
        {
            TeamId = teamId;
            TeamName = teamName;
            QuizSectionId = quizSectionId;
            QuestionId = questionId;
            Response = response;
        }
    }
}