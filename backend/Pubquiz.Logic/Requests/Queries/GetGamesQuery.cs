using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Requests.Queries
{
    /// <summary>
    /// Query to get the available Games for the <see cref="User"/>.
    /// </summary>
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "UserId")]
    public class GetGamesQuery : Query<List<Game>>
    {
        public string UserId { get; set; }

        public GetGamesQuery(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override async Task<List<Game>> DoExecute()
        {
            var userCollection = UnitOfWork.GetCollection<User>();
            var user = await userCollection.GetAsync(UserId);
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var games = gameCollection.GetAsync(user.GameRefs.Select(r => r.Id).ToArray()).Result;

            return games.ToList();
        }
    }
}