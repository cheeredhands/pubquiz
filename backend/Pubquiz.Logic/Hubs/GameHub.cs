using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Messages;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Hubs
{
    /// <summary>
    /// The GameHub is responsible for communicating relevant changes in a game.
    /// To be able to manage the connection properly without the risk of sending messages
    /// to the wrong clients, we create groups.
    /// When a quiz master registers through <see cref="QuizMasterRegistered"/> two groups
    /// are created.
    /// - a group for the quiz master
    /// - a group for the teams
    /// To isolate group names within a game, the id of the game is used as postfix. So
    /// there will be a group called quizmaster-<gameId> and a group for the teams
    /// called teams-<gameId>.
    /// Whenever a connection is lost, group membership MUST be rebuild.
    /// </summary>
    [Authorize]
    public class GameHub : Hub<IGameHub>
    {
        private readonly ILogger<GameHub> _logger;

        public GameHub(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GameHub>();
        }

        public override async Task OnConnectedAsync()
        {
            var userRole = Context.User.GetUserRole();
            var currentGameId = Context.User.GetCurrentGameId();
            switch (userRole)
            {
                case UserRole.Team:
                    var teamGroupId = Helpers.GetTeamsGroupId(currentGameId);
                    await Groups.AddToGroupAsync(Context.ConnectionId, teamGroupId);
                    break;
                case UserRole.Admin:
                    await Groups.AddToGroupAsync(Context.ConnectionId, Helpers.GetAdminGroupId());
                    break;
                case UserRole.QuizMaster:
                    var quizmasterGroupId = Helpers.GetQuizMasterGroupId(currentGameId);
                    await Groups.AddToGroupAsync(Context.ConnectionId, quizmasterGroupId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _logger.LogInformation($"User {Context.User.Identity.Name} connected with role {userRole}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userRole = Context.User.GetUserRole();
            var currentGameId = Context.User.GetCurrentGameId();
            switch (userRole)
            {
                case UserRole.Team:
                    var teamGroupId = Helpers.GetTeamsGroupId(currentGameId);
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, teamGroupId);
                    break;
                case UserRole.Admin:
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, Helpers.GetAdminGroupId());
                    break;
                case UserRole.QuizMaster:
                    var quizmasterGroupId = Helpers.GetQuizMasterGroupId(currentGameId);
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, quizmasterGroupId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await base.OnDisconnectedAsync(exception);
        }
       
    }
}