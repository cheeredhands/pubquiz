using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediatR.Pipeline;
using Pubquiz.Logic.Validation;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Handlers
{
    public class ValidationPreProcessor<TRequest> : IRequestPreProcessor<IRequest> // ValidationPreProcessor<TRequest> or else it don't get registered in DI d'oh!
    {
        private readonly IUnitOfWork _unitOfWork;

        public ValidationPreProcessor(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task Process(IRequest request, CancellationToken cancellationToken)
        {
            var entityValidator = new EntityValidator(_unitOfWork, request);
            entityValidator.CheckValidationAttributes();
            return Task.CompletedTask;
        }
    }
}