using System;

namespace Pubquiz.Logic.Messages
{
    public class TeamMembersChanged
    {
        public string GameId { get; }
        public string TeamId { get; }
        public string TeamName { get; }
        public string MemberNames { get; }

        public TeamMembersChanged(string gameId, string teamId, string teamName, string memberNames)
        {
            GameId = gameId;
            TeamId = teamId;
            TeamName = teamName;
            MemberNames = memberNames;
        }
    }
}