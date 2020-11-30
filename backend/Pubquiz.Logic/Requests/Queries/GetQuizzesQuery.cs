using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Requests.Queries
{
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "ActorId")]
    public class GetQuizzesQuery : Query<List<QuizRef>>
    {
        public string ActorId { get; set; }

        public GetQuizzesQuery(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

#pragma warning disable 1998
        protected override async Task<List<QuizRef>> DoExecute()
#pragma warning restore 1998
        {
            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            var gameCollection = UnitOfWork.GetCollection<Game>();

            var quizRefs = quizCollection.AsQueryable().Select(q => new QuizRef {Id = q.Id, Title = q.Title}).ToList();

            foreach (var quizRef in quizRefs)
            {
                quizRef.GameRefs = gameCollection.AsQueryable().Where(g => g.QuizId == quizRef.Id)
                    .Select(g => new GameRef
                        {Id = g.Id, Title = g.Title, QuizTitle = g.QuizTitle, InviteCode = g.InviteCode}).ToList();
            }

            return quizRefs;
        }
    }
}