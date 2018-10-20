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
        protected readonly IBus Bus;

        protected Command(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork)
        {
            Bus = bus;
        }

        public Task<TResponse> Execute()
        {
            CheckValidationAttributes();
            return DoExecute();
        }

        protected abstract Task<TResponse> DoExecute();
    }
}