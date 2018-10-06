using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Persistence;
using Rebus.Bus;

namespace Pubquiz.Logic
{
    /// <summary>
    /// A command request that doesn't have a return value e.g. fire-and-forget.
    /// </summary>
    public abstract class Notification : Request
    {
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly IBus Bus;

        protected Notification(IUnitOfWork unitOfWork, IBus bus)
        {
            UnitOfWork = unitOfWork;
            Bus = bus;
        }

        public Task Execute()
        {
            return DoExecute();
        }

        protected abstract Task DoExecute();
    }
}