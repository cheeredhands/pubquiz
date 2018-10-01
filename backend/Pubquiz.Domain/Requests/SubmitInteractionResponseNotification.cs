using System.Threading.Tasks;
using Pubquiz.Persistence;

namespace Pubquiz.Domain.Requests
{
    public class SubmitInteractionResponseNotification : Notification
    {
        public SubmitInteractionResponseNotification(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override Task DoExecute()
        {
            throw new System.NotImplementedException();
        }
    }
}