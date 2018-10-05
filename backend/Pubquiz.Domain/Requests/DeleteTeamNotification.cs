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
            var teamCollection = UnitOfWork.GetCollection<Team>();

            var team = await teamCollection.GetAsync(TeamId);
            if (team == null)
            {
                throw new DomainException(3, "Invalid team id.", false);
            }

            await teamCollection.DeleteAsync(TeamId);
        }
    }
}