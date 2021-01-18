using MediatR;

namespace Pubquiz.Logic.Messages
{
    public class TeamRegistered : INotification
    {
        public string TeamId { get; }
        public string GameId { get; }
        public string Name { get; }
        public string MemberNames { get; }

        public TeamRegistered(string teamId, string name, string gameId, string memberNames)
        {
            TeamId = teamId;
            Name = name;
            GameId = gameId;
            MemberNames = memberNames;
        }
    }
}