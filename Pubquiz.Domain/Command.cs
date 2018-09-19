using System.Threading.Tasks;
using Pubquiz.Repository;

namespace Pubquiz.Domain
{
    /// <summary>
    /// A command request.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public abstract class Command<TResponse> : Request
    {
        protected readonly IRepositoryFactory RepositoryFactory;

        protected Command(IRepositoryFactory repositoryFactory)
        {
            RepositoryFactory = repositoryFactory;
        }

        public Task<TResponse> Execute()
        {
            return DoExecute();
        }

        protected abstract Task<TResponse> DoExecute();
    }
}