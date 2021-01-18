using MediatR;
using Microsoft.Extensions.Logging;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Handlers
{
    public abstract class Handler
    {
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly IMediator Mediator;
        protected readonly ILoggerFactory LoggerFactory;

        protected Handler(IUnitOfWork unitOfWork, IMediator mediator,ILoggerFactory loggerFactory)
        {
            UnitOfWork = unitOfWork;
            Mediator = mediator;
            LoggerFactory = loggerFactory;
        }
    }
}