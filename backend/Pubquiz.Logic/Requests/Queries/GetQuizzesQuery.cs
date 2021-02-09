using System.Collections.Generic;
using MediatR;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;

namespace Pubquiz.Logic.Requests.Queries
{
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "ActorId")]
    public class GetQuizzesQuery : IRequest<List<QuizRef>>
    {
        public string ActorId { get; set; }
    }
}