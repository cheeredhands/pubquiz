using System.Collections.Generic;
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
    public class UserQueryHandlers : Handler, IRequestHandler<GetGamesQuery, List<Game>>,
        IRequestHandler<GetQuizzesQuery, List<QuizRef>>, IRequestHandler<QmInGameViewModelQuery, QmInGameViewModel>,
        IRequestHandler<QmLobbyViewModelQuery, QmLobbyViewModel>, IRequestHandler<QuizItemQuery, QuizItem>,
        IRequestHandler<UserQuery, User>
    {
        public UserQueryHandlers(IUnitOfWork unitOfWork, IMediator mediator, ILoggerFactory loggerFactory) : base(
            unitOfWork, mediator, loggerFactory)
        {
        }

        public async Task<List<Game>> Handle(GetGamesQuery request, CancellationToken cancellationToken)
        {
            var userCollection = UnitOfWork.GetCollection<User>();
            var user = await userCollection.GetAsync(request.UserId);
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var games = gameCollection.GetAsync(user.GameRefs.Select(r => r.Id).ToArray()).Result;

            return games.ToList();
        }

        public Task<List<QuizRef>> Handle(GetQuizzesQuery request, CancellationToken cancellationToken)
        {
            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            var gameCollection = UnitOfWork.GetCollection<Game>();

            var quizRefs = quizCollection.AsQueryable().Select(q => new QuizRef {Id = q.Id, Title = q.Title}).ToList();

            foreach (var quizRef in quizRefs)
            {
                quizRef.GameRefs = gameCollection.AsQueryable().Where(g => g.QuizId == quizRef.Id)
                    .Select(g => new GameRef
                        {Id = g.Id, Title = g.Title, QuizTitle = g.QuizTitle, InviteCode = g.InviteCode}).ToList();
            }

            return Task.FromResult(quizRefs);
        }

        public async Task<QmInGameViewModel> Handle(QmInGameViewModelQuery request, CancellationToken cancellationToken)
        {
            var userCollection = UnitOfWork.GetCollection<User>();
            var user = await userCollection.GetAsync(request.ActorId);
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(user.CurrentGameId);
            var teamCollection = UnitOfWork.GetCollection<Team>();
            //var teamViewModels = teamCollection.GetAsync(game.TeamIds.ToArray()).Result.Select(t => new TeamViewModel(t));
            var teams = await teamCollection.GetAsync(game.TeamIds.ToArray());
            var quizItemCollection = UnitOfWork.GetCollection<QuizItem>();
            var quizItem = await quizItemCollection.GetAsync(game.CurrentQuizItemId);
            var model = new QmInGameViewModel
            {
                Game = game,
                CurrentQuizItem = quizItem,
                Teams = teams.ToList()
            };

            return model;
        }

        public async Task<QmLobbyViewModel> Handle(QmLobbyViewModelQuery request, CancellationToken cancellationToken)
        {
            var userCollection = UnitOfWork.GetCollection<User>();
            var user = await userCollection.GetAsync(request.UserId);
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(user.CurrentGameId);
            var teamCollection = UnitOfWork.GetCollection<Team>();

            var teams = teamCollection.GetAsync(game.TeamIds.ToArray()).Result.ToList();
            //clear the answers per team, not needed in the lobby.
            foreach (var team in teams)
            {
                team.Answers = new Dictionary<string, Answer>();
            }

            var model = new QmLobbyViewModel
            {
                UserId = request.UserId,
                Game = game,
                TeamsInGame = teams
            };

            return model;
        }

        public async Task<QuizItem> Handle(QuizItemQuery request, CancellationToken cancellationToken)
        {
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var quizItemCollection = UnitOfWork.GetCollection<QuizItem>();


            var game = await gameCollection.GetAsync(request.GameId);

            var user = await UnitOfWork.GetCollection<User>().GetAsync(request.ActorId);

            if (user.UserRole == UserRole.Team)
            {
                if (game.CurrentQuizItemId != request.QuizItemId)
                {
                    throw new DomainException(ResultCode.TeamCantAccessQuizItemOtherThanTheCurrent,
                        "Can't access other quizitems than the current.", true);
                }
            }

            if (user.UserRole != UserRole.Admin)
            {
                if (!game.QuizMasterIds.Contains(request.ActorId))
                {
                    throw new DomainException(ResultCode.QuizMasterUnauthorizedForGame,
                        $"Actor with id {request.ActorId} is not authorized for game '{game.Id}'", true);
                }
            }

            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            var quiz = await quizCollection.GetAsync(game.QuizId);

            if (!quiz.QuizItemIds.Contains(request.QuizItemId))
            {
                throw new DomainException(ResultCode.QuizMasterUnauthorizedForQuizItem,
                    "Requested quiz item is not in any games the current user is authorized for.", true);
            }

            var quizItem = await quizItemCollection.GetAsync(request.QuizItemId);

            return quizItem;
        }

        public async Task<User> Handle(UserQuery request, CancellationToken cancellationToken)
        {
            var userCollection = UnitOfWork.GetCollection<User>();
            var user = await userCollection.GetAsync(request.UserId);
            return user;
        }
    }
}