using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pubquiz.Domain;
using Pubquiz.Logic.Requests;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Pubquiz.WebApi.Models;

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

        #region Team actions

        [HttpGet("teamlobby")]
        [Authorize(AuthPolicy.Team)]
        public async Task<IActionResult> GetTeamLobby()
        {
            var teamId = User.GetId();
            var query = new TeamLobbyViewModelQuery(_unitOfWork) {TeamId = teamId};
            var result = await query.Execute();
            return Ok(result);
        }

        [HttpPost("submitresponse")]
        [Authorize(AuthPolicy.Team)]
        public async Task<IActionResult> SubmitInteractionResponse(SubmitInteractionResponseNotification notification)
        {
            var teamId = User.GetId();
            if (!string.IsNullOrWhiteSpace(notification.TeamId) && teamId != notification.TeamId)
            {
                return Forbid();
            }

            notification.TeamId = User.GetId();
            await notification.Execute();
            return Ok(new {Code = ResultCode.InteractionResponseSubmitted, Message = "Response submitted ok."});
        }

        #endregion

        #region Admin and quiz master actions

        [HttpGet("quizmasterlobby")]
        [Authorize(AuthPolicy.QuizMaster)]
        public async Task<IActionResult> GetQuizMasterLobby()
        {
            var query = new QuizMasterLobbyViewModelQuery(_unitOfWork) {UserId = User.GetId()};
            var result = await query.Execute();
            return Ok(result);
        }

        [HttpGet("games")]
        [Authorize(AuthPolicy.QuizMaster)]
        public async Task<IActionResult> GetGames()
        {
            var query = new GetGamesQuery(_unitOfWork) {UserId = User.GetId()};
            var result = await query.Execute();
            return Ok(result);
        }

        [HttpPost("setgamestate")]
        [Authorize(AuthPolicy.QuizMaster)]
        public async Task<IActionResult> SetGameState(SetGameStateNotification notification)
        {
            var teamId = User.GetId();
            if (!string.IsNullOrWhiteSpace(notification.ActorId) && teamId != notification.ActorId)
            {
                return Forbid();
            }

            notification.ActorId = User.GetId();
            await notification.Execute();

            return Ok(new
            {
                Code = ResultCode.GameStateChanged,
                Message = $"Game state changed to {notification.NewGameState}."
            });
        }

        #endregion
    }
}