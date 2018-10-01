using System.Threading.Tasks;
using Pubquiz.Persistence;
using Pubquiz.Domain.ViewModels;

namespace Pubquiz.Domain.Requests
{
    public class TeamViewModelQuery : Query<TeamViewModel>
    {
        public TeamViewModelQuery(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override async Task<TeamViewModel> DoExecute()
        {
            return await Task.Run(() => new TeamViewModel());
        }
    }
}