using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.ViewModels;
using Pubquiz.Logic.Requests.Commands;
using Pubquiz.Logic.Requests.Notifications;
using Pubquiz.Logic.Requests.Queries;
using Pubquiz.Logic.Tools;
using Pubquiz.WebApi.Models;

namespace Pubquiz.WebApi.Controllers
{
    [Route("api/game")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GameController(IMediator bus)
        {
            _mediator = bus;
        }

        #region Team actions

        [HttpGet("teamlobby")]
        [Authorize(AuthPolicy.Team)]
        public async Task<IActionResult> GetTeamLobby()
        {
            var teamId = User.GetId();
            var query = new TeamLobbyViewModelQuery {TeamId = teamId};
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("teamingame")]
        [Authorize(AuthPolicy.Team)]
        public async Task<IActionResult> GetTeamInGame()
        {
            var query = new TeamInGameViewModelQuery {ActorId = User.GetId()};
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{gameId}/getteamquizitem/{quizItemId}")]
        [Authorize(AuthPolicy.Team)]
        public async Task<IActionResult> GetTeamQuizItem(string gameId, string quizItemId)
        {
            var query = new QuizItemViewModelQuery {ActorId = User.GetId(), GameId = gameId, QuizItemId = quizItemId};
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        #endregion

        #region Quiz master actions

        [HttpGet("quizmasterlobby")]
        [Authorize(AuthPolicy.QuizMaster)]
        public async Task<ActionResult<QmLobbyViewModel>> GetQuizMasterLobby()
        {
            var query = new QmLobbyViewModelQuery {UserId = User.GetId()};
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("quizmasteringame")]
        [Authorize(AuthPolicy.QuizMaster)]
        public async Task<ActionResult<QmInGameViewModel>> GetQuizMasterInGame()
        {
            var query = new QmInGameViewModelQuery {ActorId = User.GetId()};
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("{gameId}/setstate/{gameState}")]
        [Authorize(AuthPolicy.QuizMaster)]
        public async Task<ActionResult<ApiResponse>> SetState(string gameId, GameState gameState)
        {
            var command = new SetGameStateCommand
            {
                GameId = gameId, ActorId = User.GetId(), NewGameState = gameState
            };

            await _mediator.Send(command);

            return Ok(new ApiResponse
            {
                Code = ResultCode.Ok,
                Message = $"Game state changed to {command.NewGameState}."
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
            var command = new SetReviewCommand
            {
                ActorId = User.GetId(),
                GameId = gameId,
                SectionId = sectionId
            };
            await _mediator.Send(command);

            return Ok(new ApiResponse
            {
                Code = ResultCode.Ok,
                Message = $"Game set to review {command.SectionId}."
            });
        }

        [HttpPost("{gameId}/navigatebyoffset/{offset}")]
        [Authorize(AuthPolicy.QuizMaster)]
        public async Task<ActionResult<NavigateItemResponse>> NavigateToItemByOffset(string gameId, int offset)
        {
            var command = new NavigateToItemByOffsetCommand
            {
                ActorId = User.GetId(),
                GameId = gameId,
                Offset = offset
            };
            var result = await _mediator.Send(command);

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
            var query = new QuizItemQuery {ActorId = User.GetId(), GameId = gameId, QuizItemId = quizItemId};
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("{gameId}/select")]
        [Authorize(AuthPolicy.QuizMaster)]
        public async Task<ActionResult<SelectGameResponse>> SelectGame(string gameId)
        {
            var userId = User.GetId();
            var command = new SelectGameCommand {GameId = gameId, ActorId = userId};
            await _mediator.Send(command);
            return Ok(new SelectGameResponse
            {
                Code = ResultCode.Ok,
                Message = "Game selected",
                GameId = command.GameId
            });
        }

        #endregion

        #region Admin actions

        [HttpPost]
        [Authorize(AuthPolicy.QuizMaster)]
        public async Task<ActionResult<CreateGameResponse>> CreateGame(CreateGameRequest request)
        {
            var command = new CreateGameCommand
            {
                ActorId = User.GetId(),
                GameTitle = request.GameTitle,
                InviteCode = request.InviteCode,
                QuizId = request.QuizId
            };

            var result = await _mediator.Send(command);
            return Ok(new CreateGameResponse
            {
                Code = ResultCode.Ok,
                Message = "Game created.",
                GameId = result.Id
            });
        }
        
        [HttpDelete("{gameId}")]
        [Authorize(AuthPolicy.QuizMaster)]
        public async Task<ActionResult<SelectGameResponse>> DeleteGame(string gameId)
        {
            var userId = User.GetId();
            var command = new DeleteGameCommand {GameId = gameId, ActorId = userId};
            await _mediator.Send(command);
            return Ok(new ApiResponse
            {
                Code = ResultCode.Ok,
                Message = "Game deleted"
            });
        }

        #endregion
    }
}