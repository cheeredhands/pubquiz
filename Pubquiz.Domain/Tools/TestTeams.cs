using System;
using System.Collections.Generic;
using Pubquiz.Domain.Models;
using Pubquiz.Repository;

namespace Pubquiz.Domain.Tools
{
    public static class TestTeams
    {
        public static List<Team> GetTeams(IRepository<Team> teamRepository, Guid gameId)
        {
            var teams = new List<Team>();

            teams.Add(new Team
            {
                Name = "Team 1",
                GameId = gameId,
                SessionRecoveryCode = Helpers.GenerateSessionRecoveryCode(teamRepository, gameId),
                MemberNames = new List<string> {"member 1", "member 2", "member 3"}
            });

            teams.Add(new Team
            {
                Name = "Team 2",
                GameId = gameId,
                SessionRecoveryCode = Helpers.GenerateSessionRecoveryCode(teamRepository, gameId),
                MemberNames = new List<string> {"member 1", "member 2", "member 3"}
            });
            teams.Add(new Team
            {
                Name = "Team 3",
                GameId = gameId,
                SessionRecoveryCode = Helpers.GenerateSessionRecoveryCode(teamRepository, gameId),
                MemberNames = new List<string> {"member 1", "member 2", "member 3"}
            });

            return teams;
        }
    }
}