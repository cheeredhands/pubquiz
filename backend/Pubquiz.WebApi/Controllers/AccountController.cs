using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Requests;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Pubquiz.WebApi.Helpers;
using Pubquiz.WebApi.Models;
using Rebus.Bus;


namespace Pubquiz.WebApi.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBus _bus;

        public AccountController(IUnitOfWork unitOfWork, IBus bus)
        {
            _unitOfWork = unitOfWork;
            _bus = bus;
        }

        [HttpGet("whoami")]
        [AllowAnonymous]
        public async Task<ActionResult<WhoAmiResponse>> WhoAmI()
        {
            if (!User.Identity.IsAuthenticated) return Ok(new {UserName = ""});

            // Check if user/team still exists, otherwise sign out
            var userRole = User.GetUserRole();
            var userId = User.GetId();
            Team team = null;
            User user = null;
            if (userRole == UserRole.Team)
            {
                team = await new TeamQuery(_unitOfWork) {TeamId = userId}.Execute();
            }
            else
            {
                user = await new UserQuery(_unitOfWork) {UserId = userId}.Execute();
            }

            if (team == null && user == null)
            {
                await SignOut();
                return Ok(new WhoAmiResponse
                {
                    Code = SuccessCodes.ThatsYou,
                    Message = "",
                    UserName = ""
                });
            }

            return Ok(new WhoAmiResponse
            {
                Code = SuccessCodes.ThatsYou,
                Message = "",
                UserName = User.Identity.Name,
                UserId = User.GetId(),
                CurrentGameId = User.GetCurrentGameId(),
                UserRole = User.GetUserRole()
            });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<RegisterForGameResponse>> RegisterForGame(
            [FromBody] RegisterForGameCommand command)
        {
            var team = await command.Execute();
            await SignIn(team, team.GameId);

            return Ok(new RegisterForGameResponse
            {
                Code = SuccessCodes.TeamRegisteredAndLoggedIn,
                Message = $"Team {team.Name} registered and logged in.",
                TeamId = team.Id,
                TeamName = team.Name,
                MemberNames = team.MemberNames
            });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginCommand command)
        {
            var user = await command.Execute();
            await SignIn(user, user.CurrentGameId);
            return Ok(new LoginResponse
            {
                Code = SuccessCodes.UserLoggedIn,
                Message = $"User {user.UserName} logged in.",
                UserId = user.Id,
                UserName = user.UserName,
                GameIds = user.GameIds
            });
        }

        [HttpPost("selectgame")]
        [Authorize(Roles = "QuizMaster")]
        public async Task<ActionResult<SelectGameResponse>> SelectGame([FromBody] SelectGameCommand command)
        {
            var userId = User.GetId();
            if (command.ActorId != Guid.Empty && userId != command.ActorId)
            {
                return Forbid();
            }

            command.ActorId = userId;
            var user = await command.Execute();
            await SignOut();
            await SignIn(user, command.GameId);

            return Ok(new SelectGameResponse
            {
                Code = SuccessCodes.GameSelected,
                Message = "Game selected",
                GameId = command.GameId
            });
        }

        private async Task SignIn(User user, Guid currentGame = default(Guid))
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.UserRole.ToString()),
                new Claim("CurrentGame", currentGame.ToString())
            };

            var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(userIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        [HttpPost("testauth")]
        [Authorize(Roles = "Admin")]
        public ActionResult<TestAuthResponse> TestAuth()
        {
            var teamCollection = _unitOfWork.GetCollection<Team>();
            var teams = teamCollection.AsQueryable().ToList();

            return Ok(new TestAuthResponse
            {
                Code = SuccessCodes.AuthSuccesfullyTested,
                Message = $"Test ok. {User.Identity.Name} - {User.GetId()}",
                Teams = teams
            });
        }

        [HttpPost("changeteamname")]
        public async Task<ActionResult<ChangeTeamNameResponse>> ChangeTeamName(ChangeTeamNameCommand command)
        {
            var teamId = User.GetId();
            if (command.TeamId != Guid.Empty && teamId != command.TeamId)
            {
                return Forbid();
            }

            command.TeamId = teamId;

            var team = await command.Execute();
            await SignOut();
            await SignIn(team, team.GameId);
            return Ok(new ChangeTeamNameResponse
            {
                Code = SuccessCodes.TeamRenamed,
                Message = "Team renamed.",
                TeamName = team.Name
            });
        }

        [HttpPost("changeteammembers")]
        public async Task<ActionResult<ChangeTeamMembersResponse>> ChangeTeamMembers(ChangeTeamMembersCommand command)
        {
            var teamId = User.GetId();
            if (command.TeamId != Guid.Empty && command.TeamId != teamId)
            {
                return Forbid();
            }

            command.TeamId = teamId;

            var teamMembers = await command.Execute();
            return Ok(new ChangeTeamMembersResponse
            {
                Code = SuccessCodes.TeamMembersChanged,
                Message = "Team members changed.",
                TeamMembers = teamMembers
            });
        }

        [HttpPost("deleteteam")]
        [Authorize(Roles = "QuizMaster, Admin")]
        public async Task<ActionResult<ApiResponse>> DeleteTeam(DeleteTeamNotification notification)
        {
            var actorId = User.GetId();
            if (notification.ActorId != Guid.Empty && notification.ActorId != actorId)
            {
                return Forbid();
            }

            notification.ActorId = actorId;
            await notification.Execute();
            return Ok(new ApiResponse
            {
                Code = SuccessCodes.TeamDeleted,
                Message = $"Team with id {notification.TeamId} deleted"
            });
        }

        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse>> Logout()
        {
            var actorId = User.GetId();
            var actorRole = User.GetUserRole();

            if (actorRole == UserRole.Team)
            {
                var notification = new LogoutTeamNotification(_unitOfWork, _bus) {TeamId = actorId};
                await notification.Execute();
            }
            else
            {
                var notification = new LogoutUserNotification(_unitOfWork, _bus) {UserId = actorId};
                await notification.Execute();
            }

            await SignOut();
            return Ok(new ApiResponse
            {
                Code = SuccessCodes.LoggedOut,
                Message = "Successfully logged out."
            });
        }

        private async Task SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}