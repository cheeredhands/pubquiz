using System;
using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Pubquiz.Persistence.Extensions;
using Rebus.Bus;

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

        public CreateGameCommand(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork, bus)
        {
        }

        protected override async Task<Game> DoExecute()
        {
            // TODO check invite code    

            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            var quiz = await quizCollection.GetAsync(QuizId);

            // TODO link all quiz masters for now
            var userCollection = UnitOfWork.GetCollection<User>();
            var quizMasters = userCollection.AsQueryable().Where(u => u.UserRole == UserRole.QuizMaster);


            var game = new Game
            {
                Id = Guid.NewGuid().ToShortGuidString(),
                QuizId = QuizId,
                Title = GameTitle,
                QuizTitle = quiz.Title,
                InviteCode = InviteCode,
                QuizMasterIds = quizMasters.Select(q => q.Id).ToList(),
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
            foreach (var quizMaster in quizMasters)
            {
                var gameRef = new GameRef {Id = game.Id, Title = game.Title, QuizTitle = game.QuizTitle, InviteCode = game.InviteCode};
                quizMaster.GameRefs.Add(gameRef);
                var quizRef = quizMaster.QuizRefs.First(r => r.Id == QuizId);
                quizRef.GameRefs.Add(gameRef);
                await userCollection.UpdateAsync(quizMaster);
            }

            var gameCollection = UnitOfWork.GetCollection<Game>();
            await gameCollection.AddAsync(game);

            return game;
        }
    }
}