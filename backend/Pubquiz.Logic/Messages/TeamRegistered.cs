using System;

namespace Pubquiz.Logic.Messages
{
    public class TeamRegistered
    {
        public Guid TeamId { get; }
        public Guid GameId { get; }
        public string TeamName { get; }
        public string MemberNames { get; }

        public TeamRegistered(Guid teamId, string teamName, Guid gameId, string memberNames)
        {
            TeamId = teamId;
            TeamName = teamName;
            GameId = gameId;
            MemberNames = memberNames;
        }
    }
}