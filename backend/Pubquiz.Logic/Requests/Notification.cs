using System.Threading.Tasks;
using Pubquiz.Persistence;
using Rebus.Bus;

namespace Pubquiz.Logic.Requests
{
    /// <summary>
    /// A command request that doesn't have a return value e.g. fire-and-forget.
    /// </summary>
    public abstract class Notification : Request
    {        
        protected readonly IBus Bus;

        protected Notification(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork)
        {
            Bus = bus;
        }

        public Task Execute()
        {
            CheckValidationAttributes();
            return DoExecute();
        }

        protected abstract Task DoExecute();
    }
}