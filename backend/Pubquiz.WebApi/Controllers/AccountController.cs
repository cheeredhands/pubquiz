using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Requests;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
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
        private readonly IConfiguration _configuration;

        public AccountController(IUnitOfWork unitOfWork, IBus bus, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _bus = bus;
            _configuration = configuration;
        }

        [HttpGet("whoami")]
        [AllowAnonymous]
        public async Task<ActionResult<WhoAmiResponse>> WhoAmI()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Ok(new WhoAmiResponse
                {
                    UserName = "",
                    Code = ResultCode.LoggedOut,
                    Message = "You're not logged in",
                });
            }

            // Check if user/team still exists, otherwise sign out
            var userRole = User.GetUserRole();
            var userId = User.GetId();

            var user = userRole == UserRole.Team
                ? await new TeamQuery(_unitOfWork) {TeamId = userId}.Execute()
                : await new UserQuery(_unitOfWork) {UserId = userId}.Execute();

            if (user == null)
            {
                return Ok(new WhoAmiResponse
                {
                    UserName = "",
                    Code = ResultCode.LoggedOut,
                    Message = "You're not logged in"
                });
            }

            return Ok(new WhoAmiResponse
            {
                Code = ResultCode.ThatsYou,
                Message = "",
                UserName = user.UserName,
                UserId = User.GetId(),
                CurrentGameId = user.CurrentGameId,
                UserRole = User.GetUserRole()
            });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<RegisterForGameResponse>> RegisterForGame(
            [FromBody] RegisterForGameCommand command)
        {
            var team = await command.Execute();
            var jwt = SignInAndGetJwt(team);

            return Ok(new RegisterForGameResponse
            {
                Code = ResultCode.TeamRegisteredAndLoggedIn,
                Message = $"Team '{team.Name}' registered and logged in.",
                Jwt = jwt,
                TeamId = team.Id,
                GameId = team.CurrentGameId,
                TeamName = team.Name,
                MemberNames = team.MemberNames
            });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginCommand command)
        {
            var user = await command.Execute();
            var jwt = SignInAndGetJwt(user);
            return Ok(new LoginResponse
            {
                Jwt = jwt,
                Code = ResultCode.UserLoggedIn,
                Message = $"User {user.UserName} logged in.",
                UserId = user.Id,
                UserName = user.UserName,
                CurrentGameId = user.CurrentGameId,
                GameIds = user.GameIds
            });
        }

        [HttpPost("selectgame")]
        [Authorize(Roles = "QuizMaster")]
        public async Task<ActionResult<SelectGameResponse>> SelectGame([FromBody] SelectGameNotification notification)
        {
            var userId = User.GetId();
            if (!string.IsNullOrWhiteSpace(notification.ActorId) && userId != notification.ActorId)
            {
                return Forbid();
            }

            notification.ActorId = userId;
            await notification.Execute();

            return Ok(new SelectGameResponse
            {
                Code = ResultCode.GameSelected,
                Message = "Game selected",
                GameId = notification.GameId
            });
        }

        private string SignInAndGetJwt(User user)
        {
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["AppSettings:JwtSecret"]);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, user.UserRole.ToString()),
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpPost("testauth")]
        [Authorize(Roles = "Admin")]
        public ActionResult<TestAuthResponse> TestAuth()
        {
            var teamCollection = _unitOfWork.GetCollection<Team>();
            var teams = teamCollection.AsQueryable().ToList();

            return Ok(new TestAuthResponse
            {
                Code = ResultCode.AuthSuccessfullyTested,
                Message = $"Test ok. {User.Identity.Name} - {User.GetId()}",
                Teams = teams
            });
        }

        [HttpPost("changeteamname")]
        [Authorize(Roles = "Team")]
        public async Task<ActionResult<ChangeTeamNameResponse>> ChangeTeamName(ChangeTeamNameNotification notification)
        {
            var teamId = User.GetId();
            if (!string.IsNullOrWhiteSpace(notification.TeamId) && teamId != notification.TeamId)
            {
                return Forbid();
            }

            notification.TeamId = teamId;

            await notification.Execute();

            return Ok(new ChangeTeamNameResponse
            {
                Code = ResultCode.TeamRenamed,
                Message = "Team renamed.",
                TeamName = notification.NewName
            });
        }

        [HttpPost("changeteammembers")]
        [Authorize(Roles = "Team")]
        public async Task<ActionResult<ChangeTeamMembersResponse>> ChangeTeamMembers(
            ChangeTeamMembersNotification notification)
        {
            var teamId = User.GetId();
            notification.TeamId = teamId;

            await notification.Execute();
            return Ok(new ChangeTeamMembersResponse
            {
                Code = ResultCode.TeamMembersChanged,
                Message = "Team members changed.",
                TeamMembers = notification.TeamMembers
            });
        }

        [HttpPost("deleteteam")]
        [Authorize(Roles = "QuizMaster, Admin")]
        public async Task<ActionResult<ApiResponse>> DeleteTeam(DeleteTeamNotification notification)
        {
            var actorId = User.GetId();
            if (!string.IsNullOrWhiteSpace(notification.ActorId) && notification.ActorId != actorId)
            {
                return Forbid();
            }

            notification.ActorId = actorId;
            await notification.Execute();
            return Ok(new ApiResponse
            {
                Code = ResultCode.TeamDeleted,
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

            return Ok(new ApiResponse
            {
                Code = ResultCode.LoggedOut,
                Message = "Successfully logged out."
            });
        }
    }
}