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
using Pubquiz.Logic.Requests.Commands;
using Pubquiz.Logic.Requests.Notifications;
using Pubquiz.Logic.Requests.Queries;
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

            var gameCollection = _unitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(user.CurrentGameId);

            if (user is Team team)
            {
                return Ok(new WhoAmiResponse
                {
                    Code = ResultCode.ThatsYou,
                    Message = "Logged in as team.",
                    UserName = team.UserName,
                    UserId = User.GetId(),
                    Name = team.Name,
                    MemberNames = team.MemberNames,
                    CurrentGameId = team.CurrentGameId,
                    GameState = game.State,
                    UserRole = User.GetUserRole(),
                    RecoveryCode = team.RecoveryCode
                });
            }

            return Ok(new WhoAmiResponse
            {
                Code = ResultCode.ThatsYou,
                Message = "Logged in as user.",
                UserName = user.UserName,
                UserId = User.GetId(),
                CurrentGameId = user.CurrentGameId,
                QuizRefs = user.QuizRefs,
                GameRefs = user.GameRefs,
                GameState = game.State,
                UserRole = User.GetUserRole()
            });
        }

        [HttpPost("registerteam")]
        [AllowAnonymous]
        public async Task<ActionResult<RegisterForGameResponse>> RegisterForGame(RegisterTeamRequest request)
        {
            var command = new RegisterForGameCommand(_unitOfWork, _bus)
            {
                Name = request.Name,
                Code = request.Code
            };
            var team = await command.Execute();
            var jwt = SignInAndGetJwt(team);

            return Ok(new RegisterForGameResponse
            {
                Code = ResultCode.Ok,
                Message = $"Team '{team.Name}' registered and logged in.",
                Jwt = jwt,
                TeamId = team.Id,
                GameId = team.CurrentGameId,
                Name = team.Name,
                MemberNames = team.MemberNames,
                RecoveryCode = team.RecoveryCode
            });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            var command = new LoginCommand(_unitOfWork, _bus)
            {
                UserName = request.UserName,
                Password = request.Password
            };
            var user = await command.Execute();
            var jwt = SignInAndGetJwt(user);
            return Ok(new LoginResponse
            {
                Jwt = jwt,
                Code = ResultCode.Ok,
                Message = $"User {user.UserName} logged in.",
                UserId = user.Id,
                UserName = user.UserName,
                CurrentGameId = user.CurrentGameId,
                QuizRefs = user.QuizRefs,
                GameRefs = user.GameRefs
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
        [Authorize(AuthPolicy.Admin)]
        public ActionResult<TestAuthResponse> TestAuth()
        {
            var teamCollection = _unitOfWork.GetCollection<Team>();
            var teams = teamCollection.AsQueryable().ToList();

            return Ok(new TestAuthResponse
            {
                Code = ResultCode.Ok,
                Message = $"Test ok. {User.Identity.Name} - {User.GetId()}",
                Teams = teams
            });
        }

        [HttpPost("changeteamname")]
        [Authorize(Roles = "Team")]
        public async Task<ActionResult<ChangeTeamNameResponse>> ChangeTeamName(ChangeTeamNameRequest request)
        {
            var notification = new ChangeTeamNameNotification(_unitOfWork, _bus)
            {
                TeamId = User.GetId(),
                NewName = request.NewName
            };

            await notification.Execute();

            return Ok(new ChangeTeamNameResponse
            {
                Code = ResultCode.Ok,
                Message = "Team renamed.",
                TeamName = notification.NewName
            });
        }

        [HttpPost("changeteammembers")]
        [Authorize(AuthPolicy.Team)]
        public async Task<ActionResult<ChangeTeamMembersResponse>> ChangeTeamMembers(
            ChangeTeamMembersRequest request)
        {
            var notification = new ChangeTeamMembersNotification(_unitOfWork, _bus)
            {
                TeamId = User.GetId(),
                TeamMembers = request.TeamMembers
            };

            await notification.Execute();
            return Ok(new ChangeTeamMembersResponse
            {
                Code = ResultCode.Ok,
                Message = "Team members changed.",
                TeamMembers = notification.TeamMembers
            });
        }

        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse>> Logout()
        {
            if (User != null)
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
            }

            return Ok(new ApiResponse
            {
                Code = ResultCode.LoggedOut,
                Message = "Successfully logged out."
            });
        }
    }
}