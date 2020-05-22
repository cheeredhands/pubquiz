using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Requests.Queries
{
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "ActorId")]
    [ValidateEntity(EntityType = typeof(QuizItem), IdPropertyName = "QuizItemId")]
    [ValidateEntity(EntityType = typeof(Game), IdPropertyName = "GameId")]
    public class QuizItemQuery : Query<QuizItem>
    {
        public string ActorId { get; set; }
        public string GameId { get; set; }
        public string QuizItemId { get; set; }

        public QuizItemQuery(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override async Task<QuizItem> DoExecute()
        {
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var quizItemCollection = UnitOfWork.GetCollection<QuizItem>();


            var game = await gameCollection.GetAsync(GameId);

            var user = await UnitOfWork.GetCollection<User>().GetAsync(ActorId);

            if (user.UserRole == UserRole.Team)
            {
                if (game.CurrentQuizItemId != QuizItemId)
                {
                    throw new DomainException(ResultCode.TeamCantAccessQuizItemOtherThanTheCurrent,
                        "Can't access other quizitems than the current.", true);
                }
            }

            if (user.UserRole != UserRole.Admin)
            {
                if (game.QuizMasterIds.All(i => i != ActorId))
                {
                    throw new DomainException(ResultCode.QuizMasterUnauthorizedForGame,
                        $"Actor with id {ActorId} is not authorized for game '{game.Id}'", true);
                }
            }

            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            var quiz = await quizCollection.GetAsync(game.QuizId);

            if (!quiz.QuizItemIds.Contains(QuizItemId))
            {
                throw new DomainException(ResultCode.QuizMasterUnauthorizedForQuizItem,
                    "Requested quiz item is not in any games the current user is authorized for.", true);
            }

            var quizItem = await quizItemCollection.GetAsync(QuizItemId);

            return quizItem;
        }
    }
}