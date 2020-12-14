using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.ViewModels;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Requests.Queries
{
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

            if (game.CurrentQuizItemId != QuizItemId)
            {
                throw new DomainException(ResultCode.TeamCantAccessQuizItemOtherThanTheCurrent,
                    "Can't access other quizitems than the current.", true);
            }

            var quizItem = await quizItemCollection.GetAsync(QuizItemId);
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(ActorId);
            if (team != null)
            {
                team.Answers.TryGetValue(QuizItemId, out var answer);
                return new QuizItemViewModel(quizItem, game.State, answer);
            }

            return new QuizItemViewModel(quizItem, game.State);
        }
    }
}