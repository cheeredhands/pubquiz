using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Requests.Commands;
using Pubquiz.Logic.Requests.Queries;
using Pubquiz.Logic.Tools;
using Pubquiz.WebApi.Models;

namespace Pubquiz.WebApi.Controllers
{
    [Route("api/quiz")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuizController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(AuthPolicy.Admin)]
        public async Task<ActionResult<List<QuizRef>>> GetQuizzes()
        {
            var query = new GetQuizzesQuery {ActorId = User.GetId()};
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Upload a zip file containing an excel file and associated media files.
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        [HttpPost("uploadzippedexcelquiz")]
        [Authorize(AuthPolicy.QuizMaster)]
        [RequestSizeLimit(2147483648)]
        public async Task<IActionResult> UploadZippedExcelQuiz(IFormFile formFile)
        {
            await using var fileStream = formFile.OpenReadStream();
            var command = new ImportZippedExcelQuizCommand
            {
                ActorId = User.GetId(), FileName = formFile.Name, FileStream = fileStream
            };
            var result = await _mediator.Send(command);

            return Ok(new ImportZippedExcelQuizResponse
                {Code = ResultCode.Ok, Message = "Quiz successfully imported.", QuizRefs = result.QuizRefs});
        }
    }
}