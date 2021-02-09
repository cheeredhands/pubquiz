using MediatR;
using Pubquiz.Domain.Models;

namespace Pubquiz.Logic.Requests.Commands
{
    /// <summary>
    /// Command to login to the system.
    /// </summary>
    public class LoginCommand : IRequest<User>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}