using System;
using System.Threading.Tasks;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.Tools;
using Pubquiz.Repository;

namespace Pubquiz.Domain.Requests
{
    public class DeleteTeamNotification : Notification
    {
        public Guid TeamId { get; set; }

        public DeleteTeamNotification(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        protected override async Task DoExecute()
        {
            var teamRepo = RepositoryFactory.GetRepository<Team>();
            var userRepo = RepositoryFactory.GetRepository<User>();

            var user = await userRepo.GetAsync(TeamId);
            if (user == null)
            {
                throw new DomainException("Team/user not found.", false);
            }

            await userRepo.DeleteAsync(TeamId);
            await teamRepo.DeleteAsync(TeamId);
        }
    }
}