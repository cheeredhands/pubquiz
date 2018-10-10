using System;

namespace Pubquiz.Logic.Messages
{
    public class TeamNameUpdated
    {
        public Guid TeamId { get; }
        public Guid GameId { get; }
        public string OldTeamName { get; }
        public string TeamName { get; }

        public TeamNameUpdated(Guid teamId, Guid gameId, string oldTeamName, string teamName)
        {
            TeamId = teamId;
            GameId = gameId;
            OldTeamName = oldTeamName;
            TeamName = teamName;
        }
    }
}