using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.ViewModels;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Requests.Queries
{
    [ValidateEntity(EntityType = typeof(Team), IdPropertyName = "ActorId")]
    [ValidateEntity(EntityType = typeof(QuizItem), IdPropertyName = "QuizItemId")]
    [ValidateEntity(EntityType = typeof(Game), IdPropertyName = "GameId")]
    public class QuizItemViewModelQuery : Query<QuizItemViewModel>
    {
        public string ActorId { get; set; }
        public string GameId { get; set; }
        public string QuizItemId { get; set; }

        public QuizItemViewModelQuery(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override async Task<QuizItemViewModel> DoExecute()
        {
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var quizItemCollection = UnitOfWork.GetCollection<QuizItem>();


            var game = await gameCollection.GetAsync(GameId);

            var user = await UnitOfWork.GetCollection<User>().GetAsync(ActorId);

            if (game.CurrentQuizItemId != QuizItemId)
            {
                throw new DomainException(ResultCode.TeamCantAccessQuizItemOtherThanTheCurrent,
                    "Can't access other quizitems than the current.", true);
            }

            var quizItem = await quizItemCollection.GetAsync(QuizItemId);

            return new QuizItemViewModel(quizItem);
        }
    }
}