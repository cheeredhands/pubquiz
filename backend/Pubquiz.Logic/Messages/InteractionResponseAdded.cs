using System;

namespace Pubquiz.Logic.Messages
{
    public class InteractionResponseAdded
    {
        public Guid TeamId { get; }
        public Guid QuizSectionId { get; }
        public Guid QuestionId { get; }
        public int InteractionId { get; }

        public InteractionResponseAdded(Guid teamId, Guid quizSetId, Guid questionId, int interactionId)
        {
            TeamId = teamId;
            QuizSectionId = quizSetId;
            QuestionId = questionId;
            InteractionId = interactionId;
        }
    }
}