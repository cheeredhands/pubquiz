using MediatR;
using Pubquiz.Domain.ViewModels;

namespace Pubquiz.Logic.Messages
{
    public class GameSelected : INotification
    {
        public string UserId { get; set; }
        public string GameId { get; set; }
        public string NewGameId { get; set; }
        public QmLobbyViewModel QmLobbyViewModel { get; set; }

        public GameSelected(string userId, string gameId, string newGameId, QmLobbyViewModel qmLobbyViewModel)
        {
            UserId = userId;
            GameId = gameId;
            NewGameId = newGameId;
            QmLobbyViewModel = qmLobbyViewModel;
        }
    }
}