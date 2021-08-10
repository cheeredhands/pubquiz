using MediatR;

namespace Pubquiz.Logic.Messages
{
    public class GameDeleted : INotification
    {
        public string GameId { get; set; }

        public GameDeleted(string gameId)
        {
            GameId = gameId;
        }
    }
}