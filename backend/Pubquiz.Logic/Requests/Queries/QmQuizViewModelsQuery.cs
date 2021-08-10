using System.Collections.Generic;
using MediatR;
using Pubquiz.Domain.ViewModels;

namespace Pubquiz.Logic.Requests.Queries
{
    public class QmQuizViewModelsQuery : IRequest<List<QmQuizViewModel>>
    {
        public List<string> QuizIds { get; set; }
    }
}