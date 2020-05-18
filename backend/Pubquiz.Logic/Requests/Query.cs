using System.Threading.Tasks;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Requests
{
    /// <summary>
    /// A query request.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public abstract class Query<TResponse> : Request
    {
        protected Query(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Task<TResponse> Execute()
        {
            CheckValidationAttributes();

            return DoExecute();
        }

        protected abstract Task<TResponse> DoExecute();
    }
}