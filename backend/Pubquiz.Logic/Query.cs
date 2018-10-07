using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Persistence;

namespace Pubquiz.Logic
{
    /// <summary>
    /// A query request.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public abstract class Query<TResponse> : Request
    {
        protected readonly IUnitOfWork UnitOfWork;

        protected Query(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public Task<TResponse> Execute()
        {
            return DoExecute();
        }

        protected abstract Task<TResponse> DoExecute();
    }
}