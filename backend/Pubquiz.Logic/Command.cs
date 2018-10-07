using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Persistence;
using Rebus.Bus;

namespace Pubquiz.Logic
{
    /// <summary>
    /// A command request.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public abstract class Command<TResponse> : Request
    {
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly IBus Bus;

        protected Command(IUnitOfWork unitOfWork, IBus bus)
        {
            UnitOfWork = unitOfWork;
            Bus = bus;
        }

        public Task<TResponse> Execute()
        {
            return DoExecute();
        }

        protected abstract Task<TResponse> DoExecute();
    }
}