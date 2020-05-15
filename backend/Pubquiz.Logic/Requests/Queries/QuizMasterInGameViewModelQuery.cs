using System.Threading.Tasks;
using Pubquiz.Domain.ViewModels;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Requests.Queries
{
    public class QuizMasterInGameViewModelQuery: Query<QmInGameViewModel>
    {
        public QuizMasterInGameViewModelQuery(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override Task<QmInGameViewModel> DoExecute()
        {
            throw new System.NotImplementedException();
        }
    }
}