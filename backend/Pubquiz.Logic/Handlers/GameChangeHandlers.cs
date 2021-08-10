using System;
using System.Collections.Generic;
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
using Pubquiz.Persistence.Extensions;

namespace Pubquiz.Logic.Handlers
{
    public class GameChangeHandlers : Handler, IRequestHandler<SetGameStateCommand>,
        IRequestHandler<SetReviewCommand>, IRequestHandler<NavigateToSectionCommand, string>,
        IRequestHandler<NavigateToItemByOffsetCommand, string>, IRequestHandler<CreateGameCommand, Game>,
        IRequestHandler<DeleteGameCommand>
    {
        public GameChangeHandlers(IUnitOfWork unitOfWork, IMediator mediator, ILoggerFactory loggerFactory) : base(
            unitOfWork, mediator, loggerFactory)
        {
        }

        public async Task<Unit> Handle(SetGameStateCommand request, CancellationToken cancellationToken)
        {
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(request.GameId);

            var user = await UnitOfWork.GetCollection<User>().GetAsync(request.ActorId);

            if (user.UserRole != UserRole.Admin)
            {
                if (!game.QuizMasterIds.Contains(request.ActorId))
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
            return Unit.Value;
        }

        public async Task<Unit> Handle(SetReviewCommand request, CancellationToken cancellationToken)
        {
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(request.GameId);
            var user = await UnitOfWork.GetCollection<User>().GetAsync(request.ActorId);

            if (user.UserRole != UserRole.Admin)
            {
                if (!game.QuizMasterIds.Contains(request.ActorId))
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

            var notification = new SetGameStateCommand
            {
                ActorId = request.ActorId, GameId = request.GameId, NewGameState = GameState.Reviewing
            };
            await Mediator.Send(notification, cancellationToken);
            return Unit.Value;
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
                if (!game.QuizMasterIds.Contains(request.ActorId))
                {
                    throw new DomainException(ResultCode.QuizMasterUnauthorizedForGame,
                        $"Actor with id {request.ActorId} is not authorized for game '{game.Id}'", true);
                }
            }

            var section = quiz.QuizSections.FirstOrDefault(s => s.Id == request.SectionId);
            if (section == null)
            {
                throw new DomainException(ResultCode.InvalidEntityId,
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

        public async Task<string> Handle(NavigateToItemByOffsetCommand request, CancellationToken cancellationToken)
        {
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var quizCollection = UnitOfWork.GetCollection<Quiz>();

            var game = await gameCollection.GetAsync(request.GameId);
            var quiz = await quizCollection.GetAsync(game.QuizId);

            var user = await UnitOfWork.GetCollection<User>().GetAsync(request.ActorId);

            if (user.UserRole != UserRole.Admin)
            {
                if (!game.QuizMasterIds.Contains(request.ActorId))
                {
                    throw new DomainException(ResultCode.QuizMasterUnauthorizedForGame,
                        $"Actor with id {request.ActorId} is not authorized for game '{game.Id}'", true);
                }
            }

            // check if valid navigation
            int newSectionIndex = game.CurrentSectionIndex;
            var newQuizItemIndexInTotal = game.CurrentQuizItemIndexInTotal + request.Offset;
            var newQuizItemIndexInSection = game.CurrentQuizItemIndexInSection + request.Offset;

            if (newQuizItemIndexInTotal < 1)
            {
                newQuizItemIndexInTotal = 1;
                newSectionIndex = 1;
                game.CurrentSectionQuizItemCount = quiz.QuizSections.First().QuizItemRefs.Count;
                newQuizItemIndexInSection = 1;
            }
            else if (newQuizItemIndexInTotal > game.TotalQuizItemCount)
            {
                newQuizItemIndexInTotal = game.TotalQuizItemCount;
                newSectionIndex = quiz.QuizSections.Count;
                game.CurrentSectionQuizItemCount = quiz.QuizSections.Last().QuizItemRefs.Count;
                newQuizItemIndexInSection = quiz.QuizSections.Last().QuizItemRefs.Count;
            }
            else
            {
                while (newQuizItemIndexInSection < 1)
                {
                    newSectionIndex--;
                    game.CurrentSectionQuizItemCount = quiz.QuizSections[newSectionIndex - 1].QuizItemRefs.Count;
                    newQuizItemIndexInSection += game.CurrentSectionQuizItemCount;
                }

                while (newQuizItemIndexInSection > game.CurrentSectionQuizItemCount)
                {
                    newSectionIndex++;
                    newQuizItemIndexInSection -= game.CurrentSectionQuizItemCount;
                    game.CurrentSectionQuizItemCount = quiz.QuizSections[newSectionIndex - 1].QuizItemRefs.Count;
                }
            }

            game.CurrentSectionIndex = newSectionIndex;
            var newSection = quiz.QuizSections[newSectionIndex - 1];
            var newSectionId = newSection.Id;
            var newSectionTitle = newSection.Title;
            game.CurrentSectionId = newSectionId;
            game.CurrentSectionTitle = newSectionTitle;
            game.CurrentQuizItemIndexInSection = newQuizItemIndexInSection;
            var newQuizItemId = newSection.QuizItemRefs[newQuizItemIndexInSection - 1].Id;
            game.CurrentQuizItemId = newQuizItemId;

            var questionsInPreviousSections =
                quiz.QuizSections.Take(newSectionIndex - 1).Sum(qs => qs.QuestionItemRefs.Count);
            var questionsInSectionIncludingCurrentQuizItem = quiz.QuizSections[newSectionIndex - 1].QuizItemRefs
                .Take(newQuizItemIndexInSection).Count(qi => qi.ItemType != QuizItemType.Information);
            var newQuestionIndexInTotal = questionsInPreviousSections + questionsInSectionIncludingCurrentQuizItem;
            game.CurrentQuestionIndexInTotal = newQuestionIndexInTotal;
            game.CurrentQuizItemIndexInTotal = newQuizItemIndexInTotal;

            await gameCollection.UpdateAsync(game);

            //chuck it on the bus
            await Mediator.Publish(new ItemNavigated(request.GameId, newSectionId, newSectionTitle, newQuizItemId,
                newSectionIndex,
                newQuizItemIndexInSection, newQuizItemIndexInTotal, newQuestionIndexInTotal,
                game.CurrentSectionQuizItemCount), cancellationToken);
            return newQuizItemId;
        }

        public async Task<Game> Handle(CreateGameCommand request, CancellationToken cancellationToken)
        {
            // check invite code
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var inviteCodeInUse = await gameCollection.AnyAsync(g =>
                g.State != GameState.Finished && g.State != GameState.Closed && g.InviteCode == request.InviteCode);
            if (inviteCodeInUse)
            {
                throw new DomainException(ResultCode.InvalidCode, "Invite code is invalid.", true);
            }

            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            var quiz = await quizCollection.GetAsync(request.QuizId);

            // link quiz master identified by actorId
            var userCollection = UnitOfWork.GetCollection<User>();
            var quizMaster = await userCollection.GetAsync(request.ActorId);

            var game = new Game
            {
                Id = Guid.NewGuid().ToShortGuidString(),
                QuizId = request.QuizId,
                Title = request.GameTitle,
                QuizTitle = quiz.Title,
                InviteCode = request.InviteCode,
                QuizMasterIds = new List<string> {request.ActorId}, // quizMasters.Select(q => q.Id).ToList(),
                TotalQuestionCount = quiz.TotalQuestionCount,
                TotalQuizItemCount = quiz.TotalQuizItemCount,
                CurrentSectionQuizItemCount = quiz.QuizSections[0].QuizItemRefs.Count,
                CurrentSectionIndex = 1,
                CurrentSectionId = quiz.QuizSections[0].Id,
                CurrentSectionTitle = quiz.QuizSections[0].Title,
                CurrentQuizItemId = quiz.QuizSections[0].QuizItemRefs[0].Id,
                CurrentQuizItemIndexInSection = 1,
                CurrentQuizItemIndexInTotal = 1,
                CurrentQuestionIndexInTotal = 0,
                State = GameState.Open
            };

            // add game to quiz master
            quizMaster.GameIds.Add(game.Id);
            await userCollection.UpdateAsync(quizMaster);
            await gameCollection.AddAsync(game);
            return game;
        }

        public async Task<Unit> Handle(DeleteGameCommand request, CancellationToken cancellationToken)
        {
            var userCollection = UnitOfWork.GetCollection<User>();
            var user = await userCollection.GetAsync(request.ActorId);
            if (user.UserRole != UserRole.QuizMaster)
            {
                throw new DomainException(ResultCode.UnauthorizedRole, "You can't do that with this role.", true);
            }

            if (!user.GameIds.Contains(request.GameId))
            {
                throw new DomainException(ResultCode.QuizMasterUnauthorizedForGame,
                    $"Actor with id {request.ActorId} is not authorized for game '{request.GameId}'", true);
            }

            user.GameIds.Remove(request.GameId);
            if (user.CurrentGameId == request.GameId)
            {
                user.CurrentGameId = string.Empty;
            }

            await userCollection.UpdateAsync(user);
            
            var gameCollection = UnitOfWork.GetCollection<Game>();
            await gameCollection.DeleteAsync(request.GameId);

            await Mediator.Publish(new GameDeleted(request.GameId), cancellationToken);

            return Unit.Value;
        }
    }
}