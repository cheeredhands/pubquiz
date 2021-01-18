using MediatR;
using Pubquiz.Domain.ViewModels;

namespace Pubquiz.Logic.Messages
{
    public class GameSelected : INotification
    {
        public string GameId { get; set; }
        public string NewGameId { get; set; }
        public QmLobbyViewModel QmLobbyViewModel { get; set; }

        public GameSelected(string gameId, string newGameId, QmLobbyViewModel qmLobbyViewModel)
        {
            GameId = gameId;
            NewGameId = newGameId;
            QmLobbyViewModel = qmLobbyViewModel;
        }
    }
}