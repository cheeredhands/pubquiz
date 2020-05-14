using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.ViewModels;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Requests
{
    /// <summary>
    /// Query to get the <see cref="QuizMasterLobbyViewModel"/> for a specific <see cref="User"/>.
    /// </summary>
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "UserId")]
    public class QuizMasterLobbyViewModelQuery : Query<QmLobbyViewModel>
    {
        public string UserId { get; set; }
        public QuizMasterLobbyViewModelQuery(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        
        protected override async Task<QmLobbyViewModel> DoExecute()
        {
            var userCollection = UnitOfWork.GetCollection<User>();
            var user = await userCollection.GetAsync(UserId);
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(user.CurrentGameId);
            var teamCollection = UnitOfWork.GetCollection<Team>();

            var teams = teamCollection.GetAsync(game.TeamIds.ToArray()).Result.Select(t => new TeamViewModel(t));

            var model = new QmLobbyViewModel
            {
                UserId = UserId,
                CurrentGame = game.ToViewModel(),
                TeamsInGame = teams.ToList()
            };

            return model;
        }
    }
}