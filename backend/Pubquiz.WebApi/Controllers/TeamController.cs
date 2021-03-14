using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pubquiz.Domain;
using Pubquiz.Logic.Requests.Notifications;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Pubquiz.WebApi.Models;

namespace Pubquiz.WebApi.Controllers
{
    [Route("api/team")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public TeamController(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        [HttpDelete("{teamId}")]
        [Authorize(AuthPolicy.QuizMaster)]
        public async Task<ActionResult<ApiResponse>> DeleteTeam(string teamId)
        {
            var command = new DeleteTeamCommand {ActorId = User.GetId(), TeamId = teamId};

            await _mediator.Send(command);

            return Ok(new ApiResponse
            {
                Code = ResultCode.Ok,
                Message = $"Team with id {command.TeamId} deleted"
            });
        }

        [HttpPost("submitanswer")]
        [Authorize(AuthPolicy.Team)]
        public async Task<IActionResult> SubmitInteractionResponse(
            SubmitAnswerRequest request)
        {
            var command = new SubmitInteractionResponseCommand
            {
                TeamId = User.GetId(),
                QuizItemId = request.QuizItemId,
                InteractionId = request.InteractionId,
                Response = request.Response,
                ChoiceOptionIds = request.ChoiceOptionIds
            };
            command.TeamId = User.GetId();
            await _mediator.Send(command);
            return Ok(new ApiResponse {Code = ResultCode.Ok, Message = "Response submitted ok."});
        }

        [HttpPost("{teamId}/correction/{quizItemId}/{interactionId}/{correct}")]
        [Authorize(AuthPolicy.QuizMaster)]
        public async Task<ActionResult<ApiResponse>> CorrectInteraction(string teamId, string quizItemId,
            int interactionId, bool correct)
        {
            var command = new CorrectInteractionCommand
            {
                ActorId = User.GetId(), TeamId = teamId, QuizItemId = quizItemId, InteractionId = interactionId,
                Correct = correct
            };
            await _mediator.Send(command);
            return Ok(new ApiResponse {Code = ResultCode.Ok, Message = "Interaction corrected."});
        }
    }
}