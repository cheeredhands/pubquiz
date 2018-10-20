using System;
using System.Threading.Tasks;
using Pubquiz.Domain.Models;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Requests
{
    public class TeamQuery : Query<Team>
    {
        public Guid TeamId { get; set; }

        public TeamQuery(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override async Task<Team> DoExecute()
        {
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(TeamId);
            return team;
        }
    }
}