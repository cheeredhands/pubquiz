using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Messages;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Rebus.Bus;

namespace Pubquiz.Logic.Requests
{
    [ValidateEntity(EntityType = typeof(Game), IdPropertyName = "GameId")]
    public class NavigateToItemByOffsetCommand : Command<string>
    {
        public int Offset { get; set; }
        public string GameId { get; set; }
        public string ActorId { get; set; }

        public NavigateToItemByOffsetCommand(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork, bus)
        {
        }

        protected override async Task<string> DoExecute()
        {
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var quizCollection = UnitOfWork.GetCollection<Quiz>();

            var game = await gameCollection.GetAsync(GameId);
            var quiz = await quizCollection.GetAsync(game.QuizId);

            var user = await UnitOfWork.GetCollection<User>().GetAsync(ActorId);

            if (user.UserRole != UserRole.Admin)
            {
                if (game.QuizMasterIds.All(i => i != ActorId))
                {
                    throw new DomainException(ResultCode.QuizMasterUnauthorizedForGame,
                        $"Actor with id {ActorId} is not authorized for game '{game.Id}'", true);
                }
            }

            // check if valid navigation
            int newSectionIndex = game.CurrentSectionIndex;
            var newQuizItemIndexInTotal = game.CurrentQuizItemIndexInTotal + Offset;
            var newQuizItemIndexInSection = game.CurrentQuizItemIndexInSection + Offset;

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
                game.CurrentSectionQuizItemCount = newSectionIndex;
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
            var newSectionId = quiz.QuizSections[newSectionIndex - 1].Id;
            game.CurrentSectionId = newSectionId;
            game.CurrentQuizItemIndexInSection = newQuizItemIndexInSection;
            var newQuizItemId = quiz.QuizSections[newSectionIndex - 1].QuizItemRefs[newQuizItemIndexInSection - 1].Id;
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
            await Bus.Publish(new ItemNavigated(GameId, newSectionId, newQuizItemId, newSectionIndex,
                newQuizItemIndexInSection,
                newQuizItemIndexInTotal, newQuestionIndexInTotal, game.CurrentSectionQuizItemCount));
            return newQuizItemId;
        }
    }
}