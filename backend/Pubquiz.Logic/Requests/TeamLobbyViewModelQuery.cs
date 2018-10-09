using System;
using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.ViewModels;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Requests
{
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
            if (team == null)
            {
                throw new DomainException(ErrorCodes.InvalidTeamId, "Invalid team id, or you're not a team.", false);
            }

            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(team.GameId);

            if (game.State != GameState.Open)
            {
                throw new DomainException(ErrorCodes.LobbyUnavailableBecauseOfGameState,
                    "The lobby for this game is not open.", true);
            }

            var otherTeamsInGame = teamCollection.AsQueryable()
                .Where(t => t.Id != TeamId && game.TeamIds.Contains(t.Id))
                .Select(t => t.Name).ToList();

            var model = new TeamLobbyViewModel
            {
                TeamId = TeamId,
                TeamMembers = team.MemberNames,
                TeamName = team.Name,
                OtherTeamsInGame = otherTeamsInGame
            };
            return model;
        }
    }
}