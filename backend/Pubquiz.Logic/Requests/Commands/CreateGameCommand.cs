using MediatR;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;

namespace Pubquiz.Logic.Requests.Commands
{
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "ActorId")]
    [ValidateEntity(EntityType = typeof(Quiz), IdPropertyName = "QuizId")]
    public class CreateGameCommand : IRequest<Game>
    {
        public string ActorId { get; set; }
        public string QuizId { get; set; }
        public string InviteCode { get; set; }
        public string GameTitle { get; set; }
    }
}