using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Messages;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Rebus.Bus;

namespace Pubquiz.Logic.Requests
{
    /// <summary>
    /// Command to register for a <see cref="Game"/>.
    /// </summary>
    public class RegisterForGameCommand : Command<Team>
    {        
        public string TeamName;
        public string Code;

        public RegisterForGameCommand(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork, bus)
        {
        }

        protected override async Task<Team> DoExecute()
        {
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var teamCollection = UnitOfWork.GetCollection<Team>();

            // check validity of invite code, otherwise throw DomainException
            var game = gameCollection.AsQueryable().FirstOrDefault(g => g.InviteCode == Code);
            Team team = null;
            if (game == null)
            {
                // check if it's a recovery code for a team
                team = teamCollection.AsQueryable().FirstOrDefault(t => t.RecoveryCode == Code);
                if (team == null)
                {
                    throw new DomainException(ErrorCodes.InvalidCode, "Invalid code.", false);
                }

                if (team.IsLoggedIn)
                {
                    throw new DomainException(ErrorCodes.TeamAlreadyLoggedIn,
                        "Team is already logged in on another device.", true);
                }
            }

            // check if team name is taken, otherwise throw DomainException
            var isTeamNameTaken = !string.IsNullOrWhiteSpace(TeamName) &&
                                  await teamCollection.AnyAsync(t => t.Name == TeamName && t.GameId == game.Id);
            if (isTeamNameTaken)
            {
                throw new DomainException(ErrorCodes.TeamNameIsTaken, "Team name is taken.", true);
            }

            if (team == null)
            {
                // register team and return team object
                var userName = TeamName.Trim();
                var recoveryCode = Helpers.GenerateSessionRecoveryCode(teamCollection, game.Id);
                team = new Team
                {
                    Name = userName,
                    UserName = userName,
                    GameId = game.Id,
                    RecoveryCode = recoveryCode,
                    UserRole = UserRole.Team,
                    IsLoggedIn = true
                };
                game.TeamIds.Add(team.Id);
                await teamCollection.AddAsync(team);
                await gameCollection.UpdateAsync(game);
            }
            else
            {
                team.IsLoggedIn = true;
                team.Name = string.IsNullOrWhiteSpace(TeamName) ? team.Name : TeamName;
                await teamCollection.UpdateAsync(team);
            }

            await Bus.Publish(new TeamRegistered(team.Id, team.Name, team.GameId));

            return team;
        }
    }
}