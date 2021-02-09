using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Requests.Commands;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Handlers
{
    public class ImportQuizHandlers : Handler, IRequestHandler<ImportZippedExcelQuizCommand, QuizrPackage>
    {
        private readonly QuizrSettings _quizrSettings;

        public ImportQuizHandlers(IUnitOfWork unitOfWork, IMediator mediator, ILoggerFactory loggerFactory,
            QuizrSettings quizrSettings) : base(unitOfWork, mediator, loggerFactory)
        {
            _quizrSettings = quizrSettings;
        }

        public async Task<QuizrPackage> Handle(ImportZippedExcelQuizCommand request,
            CancellationToken cancellationToken)
        {
            var importer = new ExcelQuizImporter(UnitOfWork, LoggerFactory, _quizrSettings, request.FileStream,
                request.FileName, request.ActorId);
            return await importer.Import();
        }
    }
}