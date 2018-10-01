using System.Threading.Tasks;
using Pubquiz.Persistence;

namespace Pubquiz.Domain
{
    /// <summary>
    /// A command request that doesn't have a return value e.g. fire-and-forget.
    /// </summary>
    public abstract class Notification : Request
    {
        protected readonly IUnitOfWork UnitOfWork;

        protected Notification(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public Task Execute()
        {
            return DoExecute();
        }

        protected abstract Task DoExecute();
    }
}