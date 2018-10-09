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
            if (game == null)
            {
                // check if it's a recovery code for a team
                var team = teamCollection.AsQueryable().FirstOrDefault(t => t.RecoveryCode == Code);
                if (team != null)
                {
                    return team;
                }

                throw new DomainException(ErrorCodes.InvalidCode, "Invalid code.", false);
            }

            // check if team name is taken, otherwise throw DomainException
            var isTeamNameTaken = await teamCollection.AnyAsync(t => t.Name == TeamName && t.GameId == game.Id);
            if (isTeamNameTaken)
            {
                throw new DomainException(ErrorCodes.TeamNameIsTaken, "Team name is taken.", true);
            }

            // register team and return team object
            var userName = TeamName.ReplaceSpaces();
            var recoveryCode = Helpers.GenerateSessionRecoveryCode(teamCollection, game.Id);
            var newTeam = new Team
            {
                Name = TeamName,
                UserName = userName,
                GameId = game.Id,
                RecoveryCode = recoveryCode,
                UserRole = UserRole.Team
            };

            game.TeamIds.Add(newTeam.Id);

            await teamCollection.AddAsync(newTeam);
            await gameCollection.UpdateAsync(game);

            await Bus.Publish(new TeamRegistered(newTeam.Id, newTeam.Name));

            return newTeam;
        }
    }
}