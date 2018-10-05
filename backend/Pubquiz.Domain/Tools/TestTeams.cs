using System;
using System.Collections.Generic;
using Pubquiz.Domain.Models;
using Pubquiz.Persistence;

namespace Pubquiz.Domain.Tools
{
    public static class TestTeams
    {
        public static List<Team> GetTeams(Persistence.ICollection<Team> teamCollection, Guid gameId)
        {
            var teams = new List<Team>();

            var team1Name = "Team 1";
            var team2Name = "Team 2";
            var team3Name = "Team 3";

            var teamMembers = "Bruce Lee, Chuck Norris, Jack Bauer";

            teams.Add(new Team
            {
                Name = team1Name,
                UserName = team1Name.ReplaceSpaces(),
                GameId = gameId,
                RecoveryCode = Helpers.GenerateSessionRecoveryCode(teamCollection, gameId),
                MemberNames = teamMembers
            });

            teams.Add(new Team
            {
                Name = team2Name,
                UserName = team2Name.ReplaceSpaces(),
                GameId = gameId,
                RecoveryCode = Helpers.GenerateSessionRecoveryCode(teamCollection, gameId),
                MemberNames = teamMembers
            });
            teams.Add(new Team
            {
                Name = team3Name,
                UserName = team3Name.ReplaceSpaces(),

                GameId = gameId,
                RecoveryCode = Helpers.GenerateSessionRecoveryCode(teamCollection, gameId),
                MemberNames = teamMembers
            });

            return teams;
        }

        public static IEnumerable<User> GetUsersFromTeams(IEnumerable<Team> teams)
        {
            foreach (var team in teams)
            {
                yield return new User
                {
                    Id = team.Id,
                    UserName = team.UserName,
                    RecoveryCode = team.RecoveryCode
                };
            }
        }
    }
}