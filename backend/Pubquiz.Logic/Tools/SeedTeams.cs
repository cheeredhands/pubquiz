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
                    Id = Guid.NewGuid().ToShortGuidString(),
                    Name = team1Name,
                    UserName = team1Name.ReplaceSpaces(),
                    CurrentGameId = gameId,
                    RecoveryCode = Helpers.GenerateSessionRecoveryCode(teamCollection, gameId),
                    MemberNames = teamMembers
                },
                new Team
                {
                    Id = Guid.NewGuid().ToShortGuidString(),
                    Name = team2Name,
                    UserName = team2Name.ReplaceSpaces(),
                    CurrentGameId = gameId,
                    RecoveryCode = Helpers.GenerateSessionRecoveryCode(teamCollection, gameId),
                    MemberNames = teamMembers
                },
                new Team
                {
                    Id = Guid.NewGuid().ToShortGuidString(),
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