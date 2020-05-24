using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.ViewModels;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Requests.Queries
{
    /// <summary>
    /// Query to get the <see cref="QmLobbyViewModel"/> for a specific <see cref="User"/>.
    /// </summary>
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "UserId")]
    public class QmLobbyViewModelQuery : Query<QmLobbyViewModel>
    {
        public string UserId { get; set; }

        public QmLobbyViewModelQuery(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override async Task<QmLobbyViewModel> DoExecute()
        {
            var userCollection = UnitOfWork.GetCollection<User>();
            var user = await userCollection.GetAsync(UserId);
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(user.CurrentGameId);
            var teamCollection = UnitOfWork.GetCollection<Team>();

            var teams = teamCollection.GetAsync(game.TeamIds.ToArray()).Result.ToList();
            //clear the answers per team, not needed in the lobby.
            foreach (var team in teams)
            {
                team.Answers = new Dictionary<string, Answer>();
            }

            var model = new QmLobbyViewModel
            {
                UserId = UserId,
                Game = game,
                TeamsInGame = teams
            };

            return model;
        }
    }
}