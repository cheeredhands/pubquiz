using MediatR;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;

namespace Pubquiz.Logic.Requests.Queries
{
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "ActorId")]
    [ValidateEntity(EntityType = typeof(QuizItem), IdPropertyName = "QuizItemId")]
    [ValidateEntity(EntityType = typeof(Game), IdPropertyName = "GameId")]
    public class QuizItemQuery :  IRequest<QuizItem>
    {
        public string ActorId { get; set; }
        public string GameId { get; set; }
        public string QuizItemId { get; set; }
    }
}