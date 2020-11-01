using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pubquiz.Domain;
using Pubquiz.Logic.Requests;
using Pubquiz.Logic.Requests.Commands;
using Pubquiz.Logic.Requests.Notifications;
using Pubquiz.Logic.Requests.Queries;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Pubquiz.WebApi.Models;
using Rebus.Messages;

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
        public async Task<IActionResult> querySubmitInteractionResponse(
            SubmitInteractionResponseNotification notification)
        {
            notification.TeamId = User.GetId();
            await notification.Execute();
            return Ok(new ApiResponse {Code = ResultCode.Ok, Message = "Response submitted ok."});
        }

        [HttpGet("teamingame")]
        [Authorize(AuthPolicy.Team)]
        public async Task<IActionResult> GetTeamInGame()
        {
            var query = new TeamInGameViewModelQuery(_unitOfWork) {ActorId = User.GetId()};
            var result = await query.Execute();
            return Ok(result);
        }

        [HttpGet("{gameId}/getteamquizitem/{quizItemId}")]
        [Authorize(AuthPolicy.Team)]
        public async Task<IActionResult> GetTeamQuizItem(string gameId, string quizItemId)
        {
            var query = new QuizItemViewModelQuery(_unitOfWork)
            {
                ActorId = User.GetId(), GameId = gameId, QuizItemId = quizItemId
            };

            var result = await query.Execute();
            return Ok(result);
        }

        #endregion

        #region Admin and quiz master actions

        [HttpGet("quizmasterlobby")]
        [Authorize(AuthPolicy.QuizMaster)]
        public async Task<IActionResult> GetQuizMasterLobby()
        {
            var query = new QmLobbyViewModelQuery(_unitOfWork) {UserId = User.GetId()};
            var result = await query.Execute();
            return Ok(result);
        }

        [HttpGet("quizmasteringame")]
        [Authorize(AuthPolicy.QuizMaster)]
        public async Task<IActionResult> GetQuizMasterInGame()
        {
            var query = new QmInGameViewModelQuery(_unitOfWork) {ActorId = User.GetId()};
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

            return Ok(new ApiResponse
            {
                Code = ResultCode.Ok,
                Message = $"Game state changed to {notification.NewGameState}."
            });
        }

        [HttpPost("navigate")]
        [Authorize(AuthPolicy.QuizMaster)]
        public async Task<IActionResult> NavigateToItemByOffset(NavigateToItemByOffsetCommand command)
        {
            command.ActorId = User.GetId();
            var result = await command.Execute();

            return Ok(new NavigateItemResponse
            {
                Code = ResultCode.Ok,
                Message = "",
                QuizItemId = result
            });
        }

        [HttpGet("{gameId}/getquizitem/{quizItemId}")]
        [Authorize(AuthPolicy.QuizMaster)]
        public async Task<IActionResult> GetQuizItem(string gameId, string quizItemId)
        {
            var query = new QuizItemQuery(_unitOfWork);
            query.ActorId = User.GetId();
            query.GameId = gameId;
            query.QuizItemId = quizItemId;

            var result = await query.Execute();

            return Ok(result);
        }

        [HttpPost("correctinteraction")]
        [Authorize(AuthPolicy.QuizMaster)]
        public async Task<IActionResult> CorrectInteraction(CorrectInteractionNotification notification)
        {
            notification.ActorId = User.GetId();
            await notification.Execute();
            return Ok(new ApiResponse {Code = ResultCode.Ok, Message = "Interaction corrected."});
        }

        #endregion
    }
}