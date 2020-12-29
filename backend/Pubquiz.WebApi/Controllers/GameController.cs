using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.ViewModels;
using Pubquiz.Logic.Requests.Commands;
using Pubquiz.Logic.Requests.Notifications;
using Pubquiz.Logic.Requests.Queries;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Pubquiz.WebApi.Models;
using Rebus.Bus;

namespace Pubquiz.WebApi.Controllers
{
    [Route("api/game")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBus _bus;

        public GameController(IUnitOfWork unitOfWork, IBus bus)
        {
            _unitOfWork = unitOfWork;
            _bus = bus;
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

        #region Quiz master actions

        [HttpGet("quizmasterlobby")]
        [Authorize(AuthPolicy.QuizMaster)]
        public async Task<ActionResult<QmLobbyViewModel>> GetQuizMasterLobby()
        {
            var query = new QmLobbyViewModelQuery(_unitOfWork) {UserId = User.GetId()};
            var result = await query.Execute();
            return Ok(result);
        }

        [HttpGet("quizmasteringame")]
        [Authorize(AuthPolicy.QuizMaster)]
        public async Task<ActionResult<QmInGameViewModel>> GetQuizMasterInGame()
        {
            var query = new QmInGameViewModelQuery(_unitOfWork) {ActorId = User.GetId()};
            var result = await query.Execute();
            return Ok(result);
        }

        [HttpGet]
        [Authorize(AuthPolicy.QuizMaster)]
        public async Task<ActionResult<List<GameRef>>> GetGames()
        {
            var query = new GetGamesQuery(_unitOfWork) {UserId = User.GetId()};
            var result = await query.Execute();
            return Ok(result.Select(g => g.ToGameRef()));
        }

        [HttpPost("{gameId}/setstate/{gameState}")]
        [Authorize(AuthPolicy.QuizMaster)]
        public async Task<ActionResult<ApiResponse>> SetState(string gameId, GameState gameState)
        {
            var notification = new SetGameStateNotification(_unitOfWork, _bus)
            {
                GameId = gameId, ActorId = User.GetId(), NewGameState = gameState
            };
            await notification.Execute();

            return Ok(new ApiResponse
            {
                Code = ResultCode.Ok,
                Message = $"Game state changed to {notification.NewGameState}."
            });
        }

        /// <summary>
        /// Set the game to the review state and navigate to the first quizitem of
        /// the specified section.
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        [HttpPost("{gameId}/setreview/{sectionId}")]
        [Authorize(AuthPolicy.QuizMaster)]
        public async Task<ActionResult<ApiResponse>> SetReview(string gameId, string sectionId)
        {
            var notification = new SetReviewNotification(_unitOfWork, _bus)
            {
                ActorId = User.GetId(),
                GameId = gameId,
                SectionId = sectionId
            };
            await notification.Execute();

            return Ok(new ApiResponse
            {
                Code = ResultCode.Ok,
                Message = $"Game set to review {notification.SectionId}."
            });
        }

        [HttpPost("{gameId}/navigatebyoffset/{offset}")]
        [Authorize(AuthPolicy.QuizMaster)]
        public async Task<ActionResult<NavigateItemResponse>> NavigateToItemByOffset(string gameId, int offset)
        {
            var command = new NavigateToItemByOffsetCommand(_unitOfWork, _bus)
            {
                ActorId = User.GetId(),
                GameId = gameId,
                Offset = offset
            };
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
        public async Task<ActionResult<QuizItem>> GetQuizItem(string gameId, string quizItemId)
        {
            var query = new QuizItemQuery(_unitOfWork);
            query.ActorId = User.GetId();
            query.GameId = gameId;
            query.QuizItemId = quizItemId;

            var result = await query.Execute();

            return Ok(result);
        }

        [HttpPost("{gameId}/select")]
        [Authorize(AuthPolicy.QuizMaster)]
        public async Task<ActionResult<SelectGameResponse>> SelectGame(string gameId)
        {
            var userId = User.GetId();
            var notification = new SelectGameNotification(_unitOfWork, _bus) {GameId = gameId, ActorId = userId};
            await notification.Execute();
            return Ok(new SelectGameResponse
            {
                Code = ResultCode.Ok,
                Message = "Game selected",
                GameId = notification.GameId
            });
        }

        #endregion

        #region Admin actions

        [HttpPost]
        [Authorize(AuthPolicy.QuizMaster)]
        public async Task<ActionResult<CreateGameResponse>> CreateGame(CreateGameRequest request)
        {
            var command = new CreateGameCommand(_unitOfWork, _bus)
            {
                ActorId = User.GetId(),
                GameTitle = request.GameTitle,
                InviteCode = request.InviteCode,
                QuizId = request.QuizId
            };

            var result = await command.Execute();
            return Ok(new CreateGameResponse
            {
                Code = ResultCode.Ok,
                Message = "Game created.",
                GameId = result.Id
            });
        }

        #endregion
    }
}