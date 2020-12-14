using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.ViewModels;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Requests.Queries
{
    //[ValidateEntity(EntityType = typeof(User), IdPropertyName = "ActorId")]
    public class TeamInGameViewModelQuery: Query<TeamInGameViewModel>
    {
        public string ActorId { get; set; }
        
        public TeamInGameViewModelQuery(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override async Task<TeamInGameViewModel> DoExecute()
        {
            var userCollection = UnitOfWork.GetCollection<User>();
            var user = await userCollection.GetAsync(ActorId);
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(user.CurrentGameId);
            var quizItemCollection = UnitOfWork.GetCollection<QuizItem>();
            var quizItem = await quizItemCollection.GetAsync(game.CurrentQuizItemId);

            Answer answer = null;
            if (user.UserRole==UserRole.Team)
            {
                var teamCollection = UnitOfWork.GetCollection<Team>();
                var team = await teamCollection.GetAsync(ActorId);
                team.Answers.TryGetValue(quizItem.Id, out answer);
            }
            
            var model = new TeamInGameViewModel
            {
                Game = game,
                QuizItemViewModel = new QuizItemViewModel(quizItem, game.State, answer)
            };

            return model;
        }
    }
}