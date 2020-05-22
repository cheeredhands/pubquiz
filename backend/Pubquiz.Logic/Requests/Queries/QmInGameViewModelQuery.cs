using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.ViewModels;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Requests.Queries
{
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "ActorId")]
    public class QmInGameViewModelQuery: Query<QmInGameViewModel>
    {
        public string ActorId { get; set; }
        
        public QmInGameViewModelQuery(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override async Task<QmInGameViewModel> DoExecute()
        {
            var userCollection = UnitOfWork.GetCollection<User>();
            var user = await userCollection.GetAsync(ActorId);
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(user.CurrentGameId);
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var teams = teamCollection.GetAsync(game.TeamIds.ToArray()).Result.Select(t => new TeamViewModel(t));
            var quizItemCollection = UnitOfWork.GetCollection<QuizItem>();
            var quizItem = await quizItemCollection.GetAsync(game.CurrentQuizItemId);
            var model = new QmInGameViewModel
            {
                Game = game,
                QmTeamFeed = new QmTeamFeedViewModel(teams),
                CurrentQuizItem = quizItem,
                QmTeamRanking = new QmTeamRankingViewModel(teams)
            };

            return model;
        }
    }
}