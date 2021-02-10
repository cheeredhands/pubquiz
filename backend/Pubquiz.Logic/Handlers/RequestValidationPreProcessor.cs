using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediatR.Pipeline;
using Pubquiz.Logic.Validation;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Handlers
{
    public class RequestValidationPreProcessor<TRequest> : IRequestPreProcessor<TRequest>
        where TRequest : IBaseRequest
    {
        private readonly IUnitOfWork _unitOfWork;

        public RequestValidationPreProcessor(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var entityValidator = new EntityValidator(_unitOfWork, request);
            entityValidator.CheckValidationAttributes();
            return Task.CompletedTask;
        }
    }
}