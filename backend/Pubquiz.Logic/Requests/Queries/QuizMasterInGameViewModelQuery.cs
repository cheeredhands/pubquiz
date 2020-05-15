using System.Threading.Tasks;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.ViewModels;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Requests.Queries
{
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "ActorId")]
    public class QuizMasterInGameViewModelQuery: Query<QmInGameViewModel>
    {
        public string ActorId { get; set; }
        
        public QuizMasterInGameViewModelQuery(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override Task<QmInGameViewModel> DoExecute()
        {
            throw new System.NotImplementedException();
        }
    }
}