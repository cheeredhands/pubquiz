using MediatR;

namespace Pubquiz.Logic.Messages
{
    public class InteractionResponseAdded : INotification
    {
        public string GameId { get; }
        public string TeamId { get; }
        public string QuizItemId { get; }
        public int InteractionId { get; }
        public string Response { get; }

        public InteractionResponseAdded(string gameId, string teamId, string quizItemId, int interactionId,
            string response)
        {
            GameId = gameId;
            TeamId = teamId;
            QuizItemId = quizItemId;
            InteractionId = interactionId;
            Response = response;
        }
    }
}