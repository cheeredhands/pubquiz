using System;
using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.ViewModels;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Requests
{
    /// <summary>
    /// Query to get the <see cref="TeamLobbyViewModel"/> for a specific <see cref="Team"/>.
    /// </summary>
    [ValidateEntity(EntityType = typeof(Team), IdPropertyName = "TeamId")]
    public class TeamLobbyViewModelQuery : Query<TeamLobbyViewModel>
    {
        public Guid TeamId { get; set; }

        public TeamLobbyViewModelQuery(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override async Task<TeamLobbyViewModel> DoExecute()
        {
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(TeamId);
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(team.GameId);

            if (game.State != GameState.Open)
            {
                throw new DomainException(ErrorCodes.LobbyUnavailableBecauseOfGameState,
                    "The lobby for this game is not open.", true);
            }

            var otherTeamsInGame = teamCollection.AsQueryable()
                .Where(t => t.Id != TeamId && game.TeamIds.Contains(t.Id))
                .ToList()
                .Select(t => new TeamViewModel(t));


            var model = new TeamLobbyViewModel
            {
                TeamId = TeamId,
                Team = new TeamViewModel(team),
                OtherTeamsInGame = otherTeamsInGame.ToList()
            };
            return model;
        }
    }
}