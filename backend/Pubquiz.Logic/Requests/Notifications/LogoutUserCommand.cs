using MediatR;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;

namespace Pubquiz.Logic.Requests.Notifications
{
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "UserId")]
    public class LogoutUserCommand : IRequest
    {
        public string UserId { get; set; }
    }
}