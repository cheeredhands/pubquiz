using System;
using System.Collections.Generic;
using Pubquiz.Domain.Models;
using Pubquiz.Persistence.Extensions;

namespace Pubquiz.Logic.Tools
{
    public static class SeedTeams
    {
        public static List<Team> GetTeams(Persistence.ICollection<Team> teamCollection, string gameId)
        {
            var team1Name = "Team 1";
            var team2Name = "Team 2";
            var team3Name = "Team 3";
            var teamMembers = "Bruce Lee, Chuck Norris, Jack Bauer";
            var teams = new List<Team>
            {
                new Team
                {
                    Id = Guid.Parse("288710A4-2E9D-4C66-A88B-29FCEB624C87").ToShortGuidString(),
                    Name = team1Name,
                    UserName = team1Name.ReplaceSpaces(),
                    CurrentGameId = gameId,
                    RecoveryCode = Helpers.GenerateSessionRecoveryCode(teamCollection, gameId),
                    MemberNames = teamMembers
                },
                new Team
                {
                    Id = Guid.Parse("BC00753A-7D43-48F8-AC58-994D30FEA08C").ToShortGuidString(),
                    Name = team2Name,
                    UserName = team2Name.ReplaceSpaces(),
                    CurrentGameId = gameId,
                    RecoveryCode = Helpers.GenerateSessionRecoveryCode(teamCollection, gameId),
                    MemberNames = teamMembers
                },
                new Team
                {
                    Id = Guid.Parse("35881DC9-D8BF-4FB6-BE8D-43A65EE6EE25").ToShortGuidString(),
                    Name = team3Name,
                    UserName = team3Name.ReplaceSpaces(),
                    CurrentGameId = gameId,
                    RecoveryCode = Helpers.GenerateSessionRecoveryCode(teamCollection, gameId),
                    MemberNames = teamMembers
                }
            };

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
                    RecoveryCode = team.RecoveryCode,
                    CurrentGameId = team.CurrentGameId,
                    UserRole = UserRole.Team
                };
            }
        }
    }
}