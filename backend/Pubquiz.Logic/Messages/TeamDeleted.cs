using MediatR;

namespace Pubquiz.Logic.Messages
{
    public class TeamDeleted : INotification
    {
        public string TeamId { get; }
        public string GameId { get; }
       
        public TeamDeleted(string teamId, string gameId)
        {
            TeamId = teamId;
            GameId = gameId;
        }
    }
}