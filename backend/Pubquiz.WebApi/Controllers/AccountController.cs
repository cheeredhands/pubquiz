using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.Requests;
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

            return Ok(new {Code = 1, Message = $"Team {team.Name} registered and logged in."});
        }

        private async Task SignIn(Team team)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, team.UserName),
                new Claim(ClaimTypes.NameIdentifier, team.Id.ToString()),
                new Claim(ClaimTypes.Role, "Team")
            };
            var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(userIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        [HttpPost("testauth")]
        public ActionResult TestAuth()
        {
            var teams = new List<string>();
            var teamCollection = _unitOfWork.GetCollection<Team>();

            foreach (var team in teamCollection.AsQueryable())
            {
                teams.Add($"{team.Name} - '{team.RecoveryCode}'");
            }

            return Ok(new
            {
                Code = 42,
                Message = $"{string.Join(", ", teams.ToArray())}. Test ok. {User.Identity.Name} - {User.GetId()}"
            });
        }

        [HttpPost("changeteamname")]
        public async Task<IActionResult> ChangeTeamName(ChangeTeamNameCommand command)
        {
            var team = await command.Execute();
            await SignOut();
            await SignIn(team);
            return Ok(new {Code = 4, Message = "Team renamed."});
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await SignOut();
            return Ok(new {Code = 2, Message = "Successfully logged out."});
        }

        private async Task SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}