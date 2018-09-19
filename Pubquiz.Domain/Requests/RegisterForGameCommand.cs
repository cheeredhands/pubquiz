using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.Tools;
using Pubquiz.Repository;

namespace Pubquiz.Domain.Requests
{
    public class RegisterForGameCommand : Command<Team>
    {
        public string TeamName;
        public string Code;

        public RegisterForGameCommand(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        protected override async Task<Team> DoExecute()
        {
            var gameRepository = RepositoryFactory.GetRepository<Game>();
            var teamRepository = RepositoryFactory.GetRepository<Team>();

            // check validity of invite code, otherwise throw DomainException
            var game = gameRepository.AsQueryable().FirstOrDefault(g => g.InviteCode == Code);
            if (game == null)
            {
                // check if it's a recovery code for a team
                var team = teamRepository.AsQueryable().FirstOrDefault(t => t.SessionRecoveryCode == Code);
                if (team != null)
                {
                    return team;
                }

                throw new DomainException("Invalid code.", false);
            }

            // check if team name is taken, otherwise throw DomainException
            var isTeamNameTaken = await teamRepository.AnyAsync(t => t.Name == TeamName && t.GameId == game.Id);
            if (isTeamNameTaken)
            {
                throw new DomainException("Team name is taken.", true);
            }

            // register team and return team object
            var newTeam = new Team
            {
                Name = TeamName,
                GameId = game.Id,
                SessionRecoveryCode = Helpers.GenerateSessionRecoveryCode(teamRepository, game.Id)
            };

            await teamRepository.AddAsync(newTeam);

            return newTeam;
        }
    }
}