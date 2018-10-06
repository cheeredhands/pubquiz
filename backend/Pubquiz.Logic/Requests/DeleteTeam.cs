using System;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Persistence;
using Rebus.Bus;

namespace Pubquiz.Logic.Requests
{
    public class DeleteTeam : Notification
    {
        public Guid TeamId { get; set; }

        public DeleteTeam(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork, bus)
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