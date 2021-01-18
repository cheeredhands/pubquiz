using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Messages;
using Pubquiz.Logic.Requests.Commands;
using Pubquiz.Logic.Requests.Notifications;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Handlers
{
    public class GameChangeHandlers : Handler, INotificationHandler<SetGameStateNotification>,
        INotificationHandler<SetReviewNotification>, IRequestHandler<NavigateToSectionCommand, string>
    {
        private readonly ILogger<GameChangeHandlers> _logger;

        public GameChangeHandlers(IUnitOfWork unitOfWork, IMediator mediator, ILoggerFactory loggerFactory) : base(
            unitOfWork, mediator, loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GameChangeHandlers>();
        }

        public async Task Handle(SetGameStateNotification request, CancellationToken cancellationToken)
        {
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(request.GameId);

            var user = await UnitOfWork.GetCollection<User>().GetAsync(request.ActorId);

            if (user.UserRole != UserRole.Admin)
            {
                if (game.QuizMasterIds.All(i => i != request.ActorId))
                {
                    throw new DomainException(ResultCode.QuizMasterUnauthorizedForGame,
                        $"Actor with id {request.ActorId} is not authorized for game '{game.Id}'", true);
                }
            }

            var oldGameState = (GameState) ((int) game.State);

            game.SetState(request.NewGameState);

            await gameCollection.UpdateAsync(game);

            await Mediator.Publish(new GameStateChanged(request.GameId, oldGameState, request.NewGameState),
                cancellationToken);
        }

        public async Task Handle(SetReviewNotification request, CancellationToken cancellationToken)
        {
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(request.GameId);
            var user = await UnitOfWork.GetCollection<User>().GetAsync(request.ActorId);

            if (user.UserRole != UserRole.Admin)
            {
                if (game.QuizMasterIds.All(i => i != request.ActorId))
                {
                    throw new DomainException(ResultCode.QuizMasterUnauthorizedForGame,
                        $"Actor with id {request.ActorId} is not authorized for game '{game.Id}'", true);
                }
            }

            var command = new NavigateToSectionCommand
            {
                ActorId = request.ActorId, GameId = request.GameId, SectionId = request.SectionId
            };
            await Mediator.Send(command, cancellationToken);

            var notification = new SetGameStateNotification
            {
                ActorId = request.ActorId, GameId = request.GameId, NewGameState = GameState.Reviewing
            };
            await Mediator.Publish(notification, cancellationToken);
        }

        public async Task<string> Handle(NavigateToSectionCommand request, CancellationToken cancellationToken)
        {
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var quizCollection = UnitOfWork.GetCollection<Quiz>();

            var game = await gameCollection.GetAsync(request.GameId);
            var quiz = await quizCollection.GetAsync(game.QuizId);

            var user = await UnitOfWork.GetCollection<User>().GetAsync(request.ActorId);

            if (user.UserRole != UserRole.Admin)
            {
                if (game.QuizMasterIds.All(i => i != request.ActorId))
                {
                    throw new DomainException(ResultCode.QuizMasterUnauthorizedForGame,
                        $"Actor with id {request.ActorId} is not authorized for game '{game.Id}'", true);
                }
            }

            var section = quiz.QuizSections.FirstOrDefault(s => s.Id == request.SectionId);
            if (section == null)
            {
                throw new DomainException(ResultCode.InvalidSectionId,
                    $"Section with id {request.SectionId} doesn't exist in game with id {request.GameId}", true);
            }

            game.CurrentSectionTitle = section.Title;
            game.CurrentSectionId = section.Id;
            game.CurrentSectionIndex = quiz.QuizSections.IndexOf(section) + 1;
            game.CurrentQuizItemId = section.QuizItemRefs.First().Id;
            game.CurrentQuestionIndexInTotal = quiz.QuizSections.Take(game.CurrentSectionIndex - 1)
                .Select(s => s.QuestionItemRefs.Count).Sum() + 1;
            game.CurrentQuizItemIndexInTotal =
                quiz.QuizSections.Take(game.CurrentSectionIndex - 1).Select(s => s.QuizItemRefs.Count).Sum() + 1;
            game.CurrentSectionQuizItemCount = section.QuizItemRefs.Count;
            game.CurrentQuizItemIndexInSection = 1;

            await gameCollection.UpdateAsync(game);

            await Mediator.Publish(new ItemNavigated(request.GameId, section.Id, section.Title, game.CurrentQuizItemId,
                game.CurrentSectionIndex, game.CurrentQuizItemIndexInSection, game.CurrentQuizItemIndexInTotal,
                game.CurrentQuestionIndexInTotal, game.CurrentSectionQuizItemCount), cancellationToken);
            return game.CurrentQuizItemId;
        }
    }
}