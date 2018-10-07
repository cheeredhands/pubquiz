using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pubquiz.Logic.Requests;
using Pubquiz.WebApi.Helpers;

namespace Pubquiz.WebApi.Controllers
{
    [Route("api/game")]
    [ApiController]
    public class GameController : ControllerBase
    {
        [HttpPost("submitresponse")]
        public async Task<IActionResult> SubmitInteractionResponse(SubmitInteractionResponseNotification notification)
        {
            var teamId = User.GetId();
            if (notification.TeamId == Guid.Empty || teamId != notification.TeamId)
            {
                return Forbid();
            }

            await notification.Execute();

            return Ok(new {Code = SuccessCodes.InteractionResponseSubmitted, Message = "Response submitted ok."});
        }
    }
}