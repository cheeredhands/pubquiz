using MediatR;

namespace Pubquiz.Logic.Messages
{
    public class UserLoggedOut : INotification
    {
        public string UserId { get; }
        public string GameId { get; }
        public string UserName { get; }

        public UserLoggedOut(string userId, string userName, string gameId)
        {
            UserId = userId;
            UserName = userName;
            GameId = gameId;
        }
    }
}