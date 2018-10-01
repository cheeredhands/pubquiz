using System;
using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain.Models;
using Pubquiz.Persistence;

namespace Pubquiz.Domain.Requests
{
    public class ChangeTeamNameNotification : Notification
    {
        public Guid TeamId;
        public string NewName;

        public ChangeTeamNameNotification(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override Task DoExecute()
        {
            // check team exists
            var teamExists = UnitOfWork.GetCollection<Team>().AsQueryable().Any(t => t.Name == NewName);

            // set new name
            throw new System.NotImplementedException();
        }
    }
}