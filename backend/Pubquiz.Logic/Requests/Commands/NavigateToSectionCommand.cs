using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Messages;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Rebus.Bus;

namespace Pubquiz.Logic.Requests.Commands
{
    /// <summary>
    /// Navigate to the first item in the specified section.
    /// </summary>
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "ActorId")]
    [ValidateEntity(EntityType = typeof(Game), IdPropertyName = "GameId")]
    public class NavigateToSectionCommand : Command<string>
    {
        public string GameId { get; set; }
        public string SectionId { get; set; }
        public string ActorId { get; set; }

        public NavigateToSectionCommand(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork, bus)
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

            var section = quiz.QuizSections.FirstOrDefault(s => s.Id == SectionId);
            if (section == null)
            {
                throw new DomainException(ResultCode.InvalidSectionId,
                    $"Section with id {SectionId} doesn't exist in game with id {GameId}", true);
            }

            game.CurrentSectionTitle = section.Title;
            game.CurrentSectionId = section.Id;
            game.CurrentSectionIndex = quiz.QuizSections.IndexOf(section) + 1;
            game.CurrentQuizItemId = section.QuizItemRefs.First().Id;
            game.CurrentQuestionIndexInTotal = quiz.QuizSections.Take(game.CurrentSectionIndex)
                .Select(s => s.QuestionItemRefs.Count).Sum() + 1;
            game.CurrentQuizItemIndexInTotal =
                quiz.QuizSections.Take(game.CurrentSectionIndex).Select(s => s.QuizItemRefs.Count).Sum() + 1;
            game.CurrentSectionQuizItemCount = section.QuizItemRefs.Count;
            game.CurrentQuizItemIndexInSection = 1;

            await gameCollection.UpdateAsync(game);

            await Bus.Publish(new ItemNavigated(GameId, section.Id, section.Title, game.CurrentQuizItemId,
                game.CurrentSectionIndex, game.CurrentQuizItemIndexInSection, game.CurrentQuizItemIndexInTotal,
                game.CurrentQuestionIndexInTotal, game.CurrentSectionQuizItemCount));
            return game.CurrentQuizItemId;
        }
    }
}