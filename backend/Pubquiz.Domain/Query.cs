using System.Threading.Tasks;
using Pubquiz.Repository;

namespace Pubquiz.Domain
{
    /// <summary>
    /// A query request.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public abstract class Query<TResponse> : Request
    {
        protected readonly IRepositoryFactory RepositoryFactory;

        protected Query(IRepositoryFactory repositoryFactory)
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