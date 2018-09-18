using System.Linq;
using System.Threading.Tasks;
using Citolab.Repository;
using Pubquiz.Domain.Models;

namespace Pubquiz.Domain.Requests
{
    public class RegisterForGameCommand : Command<Team>
    {
        private readonly IRepository<Team> _teamRepository;
        private readonly IRepository<Game> _gameRepository;
        private readonly string _teamName;
        private readonly string _inviteCode;

        public RegisterForGameCommand(string teamName, string inviteCode)
        {
            _teamRepository = RepositoryFactory.GetRepository<Team>();
            _gameRepository = RepositoryFactory.GetRepository<Game>();
            _teamName = teamName;
            _inviteCode = inviteCode;
        }

        protected override Task<Team> DoExecute()
        {
            // check validity of invite code, otherwise throw DomainException
            var game = _gameRepository.AsQueryable().FirstOrDefault(g => g.InviteCode == _inviteCode);
            if (game == null)
            {
                throw new DomainException("Invalid invite code.", false);
            }

            // check if team name is taken, otherwise throw DomainException

            // register team and return team object

            throw new System.NotImplementedException();
        }
    }
}