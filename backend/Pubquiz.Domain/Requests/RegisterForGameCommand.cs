using System;
using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Domain.Requests
{
    public class RegisterForGameCommand : Command<Team>
    {
        public Guid UserId;
        public string TeamName;
        public string Code;

        public RegisterForGameCommand(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override async Task<Team> DoExecute()
        {
            var gameRepo = UnitOfWork.GetCollection<Game>();
            var teamRepo = UnitOfWork.GetCollection<Team>();
            var userRepo = UnitOfWork.GetCollection<User>();

            // check validity of invite code, otherwise throw DomainException
            var game = gameRepo.AsQueryable().FirstOrDefault(g => g.InviteCode == Code);
            if (game == null)
            {
                // check if it's a recovery code for a team
                var team = teamRepo.AsQueryable().FirstOrDefault(t => t.RecoveryCode == Code);
                if (team != null)
                {
                    return team;
                }

                throw new DomainException("Invalid code.", false);
            }

            // check if team name is taken, otherwise throw DomainException
            var isTeamNameTaken = await teamRepo.AnyAsync(t => t.Name == TeamName && t.GameId == game.Id);
            if (isTeamNameTaken)
            {
                throw new DomainException("Team name is taken.", true);
            }

            // register team and return team object
            var userName = TeamName.ReplaceSpaces();
            var normalizedUserName = userName.ToUpperInvariant();
            var recoveryCode = Helpers.GenerateSessionRecoveryCode(teamRepo, game.Id);
            var newTeam = new Team
            {
                Id = UserId,
                Name = TeamName,
                UserName = userName,
                NormalizedUserName = normalizedUserName,
                GameId = game.Id,
                RecoveryCode = recoveryCode
            };
           

            await teamRepo.AddAsync(newTeam);

            return newTeam;
        }
    }
}