using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pubquiz.Logic.Requests;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Pubquiz.WebApi.Helpers;

namespace Pubquiz.WebApi.Controllers
{
    [Route("api/game")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GameController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("teamlobby")]
        [Authorize(Roles = "Team")]
        public async Task<IActionResult> GetTeamLobby()
        {
            var teamId = User.GetId();
            var query = new TeamLobbyViewModelQuery(_unitOfWork) {TeamId = teamId};
            var result = await query.Execute();
            return Ok(result);
        }

        [HttpPost("submitresponse")]
        [Authorize(Roles = "Team")]
        public async Task<IActionResult> SubmitInteractionResponse(SubmitInteractionResponseNotification notification)
        {
            var teamId = User.GetId();
            if (notification.TeamId != Guid.Empty && teamId != notification.TeamId)
            {
                return Forbid();
            }

            notification.TeamId = User.GetId();
            await notification.Execute();
            return Ok(new {Code = SuccessCodes.InteractionResponseSubmitted, Message = "Response submitted ok."});
        }

        [HttpPost("setgamestate")]
        [Authorize(Roles = "Admin, QuizMaster")]
        public async Task<IActionResult> SetGameState(SetGameStateNotification notification)
        {
            var teamId = User.GetId();
            if (notification.ActorId != Guid.Empty && teamId != notification.ActorId)
            {
                return Forbid();
            }

            notification.ActorId = User.GetId();
            await notification.Execute();

            return Ok(new
            {
                Code = SuccessCodes.GameStateChanged,
                Message = $"Game state changed to {notification.NewGameState}."
            });
        }
    }
}