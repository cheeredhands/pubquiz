using System;
using System.Threading.Tasks;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Domain.Requests
{
    public class DeleteTeamNotification : Notification
    {
        public Guid TeamId { get; set; }

        public DeleteTeamNotification(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override async Task DoExecute()
        {
            var teamRepo = UnitOfWork.GetCollection<Team>();
            var userRepo = UnitOfWork.GetCollection<User>();

            var user = await userRepo.GetAsync(TeamId);
            if (user == null)
            {
                throw new DomainException("Team/user not found.", false);
            }

            await userRepo.DeleteAsync(TeamId);
            await teamRepo.DeleteAsync(TeamId);
            UnitOfWork.Commit();
        }
    }
}