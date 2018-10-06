using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Persistence;

namespace Pubquiz.Logic
{
    /// <summary>
    /// A command request.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public abstract class Command<TResponse> : Request
    {
        protected readonly IUnitOfWork UnitOfWork;

        protected Command(IUnitOfWork unitOfWork)
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