using System.Threading.Tasks;
using Pubquiz.Repository;
using Pubquiz.Domain.ViewModels;

namespace Pubquiz.Domain.Requests
{
    public class TeamViewModelQuery : Query<TeamViewModel>
    {
        public TeamViewModelQuery(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        protected override async Task<TeamViewModel> DoExecute()
        {
            return new TeamViewModel();
            
        }
    }
}