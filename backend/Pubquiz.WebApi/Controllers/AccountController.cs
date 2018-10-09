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
using Pubquiz.Persistence;
using Pubquiz.WebApi.Helpers;


namespace Pubquiz.WebApi.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> RegisterForGame([FromBody] RegisterForGameCommand command)
        {
            var team = await command.Execute();
            await SignIn(team);

            return Ok(new
            {
                Code = SuccessCodes.TeamRegisteredAndLoggedIn,
                Message = $"Team {team.Name} registered and logged in.",
                TeamId = team.Id
            });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var user = await command.Execute();
            await SignIn(user);
            return Ok(new
                {Code = SuccessCodes.UserLoggedIn, Message = $"User {user.UserName} logged in.", UserId = user.Id});
        }

        private async Task SignIn(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.UserRole.ToString())
            };
            var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(userIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        [HttpPost("testauth")]
        [Authorize(Roles = "Admin")]
        public ActionResult TestAuth()
        {
            var teamCollection = _unitOfWork.GetCollection<Team>();
            var teams = teamCollection.AsQueryable().ToList();

            return Ok(new
            {
                Code = SuccessCodes.AuthSuccesfullyTested,
                Message = $"Test ok. {User.Identity.Name} - {User.GetId()}",
                Teams = teams
            });
        }

        [HttpPost("changeteamname")]
        public async Task<IActionResult> ChangeTeamName(ChangeTeamNameCommand command)
        {
            var teamId = User.GetId();
            if (command.TeamId != Guid.Empty && teamId != command.TeamId)
            {
                return Forbid();
            }

            command.TeamId = teamId;

            var team = await command.Execute();
            await SignOut();
            await SignIn(team);
            return Ok(new {Code = SuccessCodes.TeamRenamed, Message = "Team renamed.", TeamName = team.Name});
        }

        [HttpPost("changeteammembers")]
        public async Task<IActionResult> ChangeTeamMembers(ChangeTeamMembersNotification notification)
        {
            var teamId = User.GetId();
            if (notification.TeamId != Guid.Empty && notification.TeamId != teamId)
            {
                return Forbid();
            }

            notification.TeamId = teamId;

            await notification.Execute();
            return Ok(new {Code = SuccessCodes.TeamMembersChanged, Message = "Team members changed."});
        }

        [HttpPost("deleteteam")]
        [Authorize(Roles = "QuizMaster, Admin")]
        public async Task<IActionResult> DeleteTeam(DeleteTeamNotification notification)
        {
            var actorId = User.GetId();
            if (notification.ActorId != Guid.Empty && notification.ActorId != actorId)
            {
                return Forbid();
            }

            notification.ActorId = actorId;
            await notification.Execute();
            return Ok(new {Code = SuccessCodes.TeamDeleted, Message = $"Team with id {notification.TeamId} deleted"});
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await SignOut();
            return Ok(new {Code = SuccessCodes.LoggedOut, Message = "Successfully logged out."});
        }

        private async Task SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}