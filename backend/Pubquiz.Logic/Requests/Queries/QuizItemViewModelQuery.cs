using MediatR;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.ViewModels;
using Pubquiz.Logic.Tools;

namespace Pubquiz.Logic.Requests.Queries
{
    [ValidateEntity(EntityType = typeof(QuizItem), IdPropertyName = "QuizItemId")]
    [ValidateEntity(EntityType = typeof(Game), IdPropertyName = "GameId")]
    public class QuizItemViewModelQuery : IRequest<QuizItemViewModel>
    {
        public string ActorId { get; set; }
        public string GameId { get; set; }
        public string QuizItemId { get; set; }
    }
}