using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Pubquiz.Persistence.Extensions;

namespace Pubquiz.Logic.Requests.Commands
{
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "ActorId")]
    [ValidateEntity(EntityType = typeof(Quiz), IdPropertyName = "QuizId")]
    public class CreateGameCommand : Command<Game>
    {
        public string ActorId { get; set; }
        public string QuizId { get; set; }
        public string InviteCode { get; set; }
        public string GameTitle { get; set; }

        public CreateGameCommand(IUnitOfWork unitOfWork, IMediator mediator) : base(unitOfWork, mediator)
        {
        }

        protected override async Task<Game> DoExecute()
        {
            // check invite code
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var inviteCodeInUse = await gameCollection.AnyAsync(g =>
                g.State != GameState.Finished && g.State != GameState.Closed && g.InviteCode == InviteCode);
            if (inviteCodeInUse)
            {
                throw new DomainException(ResultCode.InvalidCode, "Invite code is invalid.", true);
            }

            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            var quiz = await quizCollection.GetAsync(QuizId);

            // link quiz master identified by actorId
            var userCollection = UnitOfWork.GetCollection<User>();
            var quizMaster = await userCollection.GetAsync(ActorId);
            
            var game = new Game
            {
                Id = Guid.NewGuid().ToShortGuidString(),
                QuizId = QuizId,
                Title = GameTitle,
                QuizTitle = quiz.Title,
                InviteCode = InviteCode,
                QuizMasterIds = new List<string> {ActorId}, // quizMasters.Select(q => q.Id).ToList(),
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
            var gameRef = new GameRef
                {Id = game.Id, Title = game.Title, QuizTitle = game.QuizTitle, InviteCode = game.InviteCode};
            quizMaster.GameRefs.Add(gameRef);
            var quizRef = quizMaster.QuizRefs.First(r => r.Id == QuizId);
            quizRef.GameRefs.Add(gameRef);
            await userCollection.UpdateAsync(quizMaster);
            await gameCollection.AddAsync(game);
            return game;
        }
    }
}