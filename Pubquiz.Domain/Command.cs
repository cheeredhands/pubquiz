using System.Threading.Tasks;
using Citolab.Repository;

namespace Pubquiz.Domain
{
    /// <summary>
    /// A command request.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public abstract class Command<TResponse> : Request
    {
        public IRepositoryFactory RepositoryFactory { protected get; set; }

        public Task<TResponse> Execute()
        {
            return DoExecute();
        }

        protected abstract Task<TResponse> DoExecute();
    }
}