using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.ViewModels;
using Pubquiz.Logic.Requests.Queries;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Handlers
{
    public class TeamQueryHandlers : Handler, IRequestHandler<QuizItemViewModelQuery, QuizItemViewModel>,
        IRequestHandler<TeamInGameViewModelQuery, TeamInGameViewModel>,
        IRequestHandler<TeamLobbyViewModelQuery, TeamLobbyViewModel>, IRequestHandler<TeamQuery, Team>
    {
        public TeamQueryHandlers(IUnitOfWork unitOfWork, IMediator mediator, ILoggerFactory loggerFactory) : base(
            unitOfWork, mediator, loggerFactory)
        {
        }

        public async Task<QuizItemViewModel> Handle(QuizItemViewModelQuery request, CancellationToken cancellationToken)
        {
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var quizItemCollection = UnitOfWork.GetCollection<QuizItem>();


            var game = await gameCollection.GetAsync(request.GameId);

            if (game.CurrentQuizItemId != request.QuizItemId)
            {
                throw new DomainException(ResultCode.TeamCantAccessQuizItemOtherThanTheCurrent,
                    "Can't access other quizitems than the current.", true);
            }

            var quizItem = await quizItemCollection.GetAsync(request.QuizItemId);
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(request.ActorId);
            if (team != null)
            {
                team.Answers.TryGetValue(request.QuizItemId, out var answer);
                return new QuizItemViewModel(quizItem, game.State, answer);
            }

            return new QuizItemViewModel(quizItem, game.State);
        }

        public async Task<TeamInGameViewModel> Handle(TeamInGameViewModelQuery request,
            CancellationToken cancellationToken)
        {
            var userCollection = UnitOfWork.GetCollection<User>();
            var user = await userCollection.GetAsync(request.ActorId);
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(user.CurrentGameId);
            var quizItemCollection = UnitOfWork.GetCollection<QuizItem>();
            var quizItem = await quizItemCollection.GetAsync(game.CurrentQuizItemId);

            Answer answer = null;
            if (user.UserRole == UserRole.Team)
            {
                var teamCollection = UnitOfWork.GetCollection<Team>();
                var team = await teamCollection.GetAsync(request.ActorId);
                team.Answers.TryGetValue(quizItem.Id, out answer);
            }

            var model = new TeamInGameViewModel
            {
                Game = game,
                QuizItemViewModel = new QuizItemViewModel(quizItem, game.State, answer)
            };

            return model;
        }

        public async Task<TeamLobbyViewModel> Handle(TeamLobbyViewModelQuery request,
            CancellationToken cancellationToken)
        {
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(request.TeamId);
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(team.CurrentGameId);

            if (game.State == GameState.Closed || game.State == GameState.Finished)
            {
                throw new DomainException(ResultCode.LobbyUnavailableBecauseOfGameState,
                    "The lobby for this game is not open.", true);
            }

            var otherTeamsInGame = teamCollection.AsQueryable()
                .Where(t => t.Id != request.TeamId && game.TeamIds.Contains(t.Id))
                .ToList()
                .Select(t => new TeamViewModel(t));


            var model = new TeamLobbyViewModel
            {
                Game = game,
                OtherTeamsInGame = otherTeamsInGame.ToList()
            };
            return model;
        }

        public async Task<Team> Handle(TeamQuery request, CancellationToken cancellationToken)
        {
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(request.TeamId);
            return team;
        }
    }
}