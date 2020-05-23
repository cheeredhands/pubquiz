using System;

namespace Pubquiz.Logic.Messages
{
    public class TeamLoggedOut
    {
        public string TeamId { get; }
        public string GameId { get; }
        public string Name { get; }

        public TeamLoggedOut(string teamId, string name, string gameId)
        {
            TeamId = teamId;
            Name = name;
            GameId = gameId;
        }
    }
}