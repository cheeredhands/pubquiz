using System;

namespace Pubquiz.Logic.Messages
{
    public class TeamMembersChanged
    {
        public Guid GameId { get; }
        public Guid TeamId { get; }
        public string TeamName { get; }
        public string MemberNames { get; }

        public TeamMembersChanged(Guid gameId, Guid teamId, string teamName, string memberNames)
        {
            GameId = gameId;
            TeamId = teamId;
            TeamName = teamName;
            MemberNames = memberNames;
        }
    }
}