using MediatR;
using Pubquiz.Domain.Models;

namespace Pubquiz.Logic.Requests.Commands
{
    /// <summary>
    /// Command to register for a <see cref="Game"/>.
    /// </summary>
    public class RegisterForGameCommand : IRequest<Team>
    {
        public string Name;
        public string Code;
    }
}