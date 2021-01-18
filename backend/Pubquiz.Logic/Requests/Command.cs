using System.Threading.Tasks;
using MediatR;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Requests
{
    /// <summary>
    /// A command request.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public abstract class Command<TResponse> : Request
    {
        protected readonly IMediator Mediator;

        protected Command(IUnitOfWork unitOfWork, IMediator mediator) : base(unitOfWork)
        {
            Mediator = mediator;
        }

        public Task<TResponse> Execute()
        {
            CheckValidationAttributes();
            return DoExecute();
        }

        protected abstract Task<TResponse> DoExecute();
    }
}