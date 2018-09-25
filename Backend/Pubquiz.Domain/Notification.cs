using System.Threading.Tasks;
using Pubquiz.Repository;

namespace Pubquiz.Domain
{
    /// <summary>
    /// A command request that doesn't have a return value e.g. fire-and-forget.
    /// </summary>
    public abstract class Notification : Request
    {
        protected readonly IRepositoryFactory RepositoryFactory;

        protected Notification(IRepositoryFactory repositoryFactory)
        {
            RepositoryFactory = repositoryFactory;
        }

        public Task Execute()
        {
            return DoExecute();
        }

        protected abstract Task DoExecute();
    }
}