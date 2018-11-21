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
                .Select(t => new TeamViewModel
                {
                    TeamId = t.Id,
                    TeamName = t.Name, 
                    MemberNames = t.MemberNames
                })
                .ToList();

            var model = new TeamLobbyViewModel
            {
                TeamId = TeamId,
                Team = new TeamViewModel { TeamName = team.Name, MemberNames = team.MemberNames},
                OtherTeamsInGame = otherTeamsInGame
            };
            return model;
        }
    }
}