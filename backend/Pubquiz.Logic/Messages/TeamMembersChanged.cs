using System;
using MediatR;

namespace Pubquiz.Logic.Messages
{
    public class TeamMembersChanged : INotification
    {
        public string GameId { get; }
        public string TeamId { get; }
        public string Name { get; }
        public string MemberNames { get; }

        public TeamMembersChanged(string gameId, string teamId, string name, string memberNames)
        {
            GameId = gameId;
            TeamId = teamId;
            Name = name;
            MemberNames = memberNames;
        }
    }
}