using System;
using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Messages;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Rebus.Bus;

namespace Pubquiz.Logic.Requests.Commands
{
    /// <summary>
    /// Command to register for a <see cref="Game"/>.
    /// </summary>
    public class RegisterForGameCommand : Command<Team>
    {
        public string Name;
        public string Code;

        public RegisterForGameCommand(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork, bus)
        {
        }

        protected override async Task<Team> DoExecute()
        {
            var gameCollection = UnitOfWork.GetCollection<Game>();
            

            // check validity of invite code, otherwise throw DomainException
            var game = gameCollection.AsQueryable().FirstOrDefault(g => g.InviteCode == Code);

            if (game != null && (game.State == GameState.Closed || game.State == GameState.Finished))
            {
                throw new DomainException(ResultCode.InvalidCode, "Invalid or expired code.", true);
            }

            var teamCollection = UnitOfWork.GetCollection<Team>();
            Team team = null;
            if (game == null)
            {
                // check if it's a recovery code for a team
                team = teamCollection.AsQueryable().FirstOrDefault(t => t.RecoveryCode == Code);
                if (team == null)
                {
                    throw new DomainException(ResultCode.InvalidCode, "Invalid code.", false);
                }

            }

            if (team == null)
            {
                // check if team name is taken, otherwise throw DomainException
                var isTeamNameTaken = !string.IsNullOrWhiteSpace(Name) &&
                                      teamCollection.AsQueryable().ToList().Any(t =>
                                          string.Equals(t.Name, Name, StringComparison.InvariantCultureIgnoreCase) &&
                                          t.CurrentGameId == game.Id);
                if (isTeamNameTaken)
                {
                    throw new DomainException(ResultCode.TeamNameIsTaken, "Team name is taken.", true);
                }

                // register team and return team object
                var userName = Name.Trim();
                var recoveryCode = Helpers.GenerateSessionRecoveryCode(teamCollection, game.Id);

                team = new Team
                {
                    Name = userName,
                    UserName = userName,
                    CurrentGameId = game.Id,
                    RecoveryCode = recoveryCode,
                    UserRole = UserRole.Team
                };
                var user = new User
                {
                    Id = team.Id,
                    UserName = userName,
                    CurrentGameId = game.Id,
                    RecoveryCode = recoveryCode,
                    UserRole = UserRole.Team
                };
                
                game.TeamIds.Add(team.Id);
                await teamCollection.AddAsync(team);
                var userCollection = UnitOfWork.GetCollection<User>();
                await userCollection.AddAsync(user);
                await gameCollection.UpdateAsync(game);
            }
            else
            {
                team.Name = string.IsNullOrWhiteSpace(Name) ? team.Name : Name;
                await teamCollection.UpdateAsync(team);
            }

            await Bus.Publish(new TeamRegistered(team.Id, team.Name, team.CurrentGameId, team.MemberNames));
            await Bus.Publish(new QmTeamRegistered {GameId = team.CurrentGameId, Team = team});
            return team;
        }
    }
}