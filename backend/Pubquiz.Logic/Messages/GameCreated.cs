using MediatR;
using Pubquiz.Domain.ViewModels;

namespace Pubquiz.Logic.Messages
{
    public class GameCreated : INotification
    {
        public string UserId { get; }
        public QmGameViewModel QmGameViewModel { get; }

        public GameCreated(string userId, QmGameViewModel qmGameViewModel)
        {
            UserId = userId;
            QmGameViewModel = qmGameViewModel;
        }
    }
}