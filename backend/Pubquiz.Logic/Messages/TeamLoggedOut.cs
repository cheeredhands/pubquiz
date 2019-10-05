using System;

namespace Pubquiz.Logic.Messages
{
    public class TeamLoggedOut
    {
        public string TeamId { get; }
        public string GameId { get; }
        public string TeamName { get; }

        public TeamLoggedOut(string teamId, string teamName, string gameId)
        {
            TeamId = teamId;
            TeamName = teamName;
            GameId = gameId;
        }
    }
}