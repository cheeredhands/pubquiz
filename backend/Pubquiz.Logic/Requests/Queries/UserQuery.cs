using MediatR;
using Pubquiz.Domain.Models;

namespace Pubquiz.Logic.Requests.Queries
{
    public class UserQuery : IRequest<User>
    {
        public string UserId { get; set; }
    }
}