using System;

namespace Pubquiz.Logic.Messages
{
    public class TeamMembersChanged
    {
        public Guid GameId { get; }
        public Guid TeamId { get; }
        public string TeamName { get; }
        public string TeamMembers { get; }

        public TeamMembersChanged(Guid gameId, Guid teamId, string teamName, string teamMembers)
        {
            GameId = gameId;
            TeamId = teamId;
            TeamName = teamName;
            TeamMembers = teamMembers;
        }
    }
}