using System;

namespace Pubquiz.Logic.Messages
{
    public class InteractionResponseAdded
    {
        public Guid TeamId { get; }
        public string TeamName { get; }
        public Guid QuizSectionId { get; }
        public Guid QuestionId { get; }
        public string Response { get; }

        public InteractionResponseAdded(Guid teamId, string teamName, Guid quizSetId, Guid questionId, string response)
        {
            TeamId = teamId;
            TeamName = teamName;
            QuizSectionId = quizSetId;
            QuestionId = questionId;
            Response = response;
        }
    }
}