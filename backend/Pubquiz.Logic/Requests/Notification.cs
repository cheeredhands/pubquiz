using System.Threading.Tasks;
using MediatR;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Requests
{
    /// <summary>
    /// A command request that doesn't have a return value e.g. fire-and-forget.
    /// </summary>
    public abstract class Notification : Request
    {        
        protected readonly IMediator Mediator;

        protected Notification(IUnitOfWork unitOfWork, IMediator mediator) : base(unitOfWork)
        {
            Mediator = mediator;
        }

        public Task Execute()
        {
            CheckValidationAttributes();
            return DoExecute();
        }

        protected abstract Task DoExecute();
    }
}