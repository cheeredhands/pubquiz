using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pubquiz.Domain.Requests;
using Pubquiz.Domain.Tools;
using Pubquiz.Repository;
using Pubquiz.WebApi.Helpers;


namespace Pubquiz.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(IRepositoryFactory repositoryFactory, SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _repositoryFactory = repositoryFactory;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> RegisterForGame([FromBody] RegisterForGameCommand command)
        {
            //await Logout();
            //var team = command.Execute().Result;
            
            var userName = command.TeamName.ReplaceSpaces();
            var applicationUser = new ApplicationUser
            {
                TeamName = command.TeamName,
                UserName = userName,
                NormalizedUserName = userName.ToUpperInvariant(),
                Code = command.Code
            };
            var result = await _userManager.CreateAsync(applicationUser);
            if (result.Succeeded)
            {                
                await _signInManager.SignInAsync(applicationUser, true);
                return Ok();
            }
            else
            {
                return BadRequest(result);
            }
            //await HttpContext.SignInAsync(new GenericPrincipal(new GenericIdentity(command.TeamName), null));
        }

        [HttpPost("testauth")]
        public ActionResult TestAuth()
        {
            return Ok("test ok.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/swagger");
        }
    }
}