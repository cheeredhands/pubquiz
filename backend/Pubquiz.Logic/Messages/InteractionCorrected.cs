using MediatR;

namespace Pubquiz.Logic.Messages
{
    public class InteractionCorrected : INotification
    {
        public string GameId { get; }
        public string TeamId { get; }
        public string QuizItemId { get; }
        public int InteractionId { get; }
        public bool Outcome { get; }

        public InteractionCorrected(string gameId, string teamId, string quizItemId, int interactionId, bool outcome)
        {
            GameId = gameId;
            TeamId = teamId;
            QuizItemId = quizItemId;
            InteractionId = interactionId;
            Outcome = outcome;
        }
    }
}