using System.Threading.Tasks;
using Pubquiz.Repository;

namespace Pubquiz.Domain.Requests
{
    public class SubmitInteractionResponseNotification : Notification
    {
        public SubmitInteractionResponseNotification(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        protected override Task DoExecute()
        {
            throw new System.NotImplementedException();
        }
    }
}