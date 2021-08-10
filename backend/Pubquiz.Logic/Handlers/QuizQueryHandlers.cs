using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.ViewModels;
using Pubquiz.Logic.Requests.Queries;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Handlers
{
    public class QuizQueryHandlers: Handler, IRequestHandler<QmQuizViewModelsQuery, List<QmQuizViewModel>>
    {
        public QuizQueryHandlers(IUnitOfWork unitOfWork, IMediator mediator, ILoggerFactory loggerFactory) : base(unitOfWork, mediator, loggerFactory)
        {
        }

        public async Task<List<QmQuizViewModel>> Handle(QmQuizViewModelsQuery request, CancellationToken cancellationToken)
        {
            var gameCollection = UnitOfWork.GetCollection<Quiz>();
            var quizzes = await gameCollection.GetAsync(request.QuizIds.ToArray());

            return quizzes.Select(q => q.ToQmQuizViewModel()).ToList();
        }
    }
}