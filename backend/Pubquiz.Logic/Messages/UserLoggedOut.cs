using System;

namespace Pubquiz.Logic.Messages
{
    public class UserLoggedOut
    {
        public Guid UserId { get; }
        public Guid GameId { get; }
        public string UserName { get; }

        public UserLoggedOut(Guid userId, string userName, Guid gameId)
        {
            UserId = userId;
            UserName = userName;
            GameId = gameId;
        }
    }
}