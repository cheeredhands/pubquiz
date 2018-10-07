using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.ViewModels;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Requests
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