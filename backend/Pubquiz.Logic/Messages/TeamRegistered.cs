using System;
using System.Collections.Generic;
using System.Text;

namespace Pubquiz.Logic.Messages
{
    public class TeamRegistered
    {
        public Guid TeamId { get; }
        public string TeamName { get; }

        public TeamRegistered(Guid teamId, string teamName)
        {
            TeamId = teamId;
            TeamName = teamName;
        }
    }
}