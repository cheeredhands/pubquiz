using System;
using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain.Models;
using Pubquiz.Repository;

namespace Pubquiz.Domain.Requests
{
    public class ChangeTeamNameNotification : Notification
    {
        public Guid TeamId;
        public string NewName;

        public ChangeTeamNameNotification(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        protected override Task DoExecute()
        {
            // check team exists
            var teamExists = RepositoryFactory.GetRepository<Team>().AsQueryable().Any(t => t.Name == NewName);

            // set new name
            throw new System.NotImplementedException();
        }
    }
}