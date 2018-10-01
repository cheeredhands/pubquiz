using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Domain.Requests
{
    public class GetUserByNormalizedUserNameQuery : Query<User>
    {
        public string NormalizedUserName { get; set; }

        public GetUserByNormalizedUserNameQuery(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override Task<User> DoExecute() => Task.Run(() =>
        {
            var userRepo = UnitOfWork.GetCollection<User>();

            var user = userRepo.AsQueryable().FirstOrDefault(u => u.NormalizedUserName == NormalizedUserName);
            if (user == null)
            {
                throw new DomainException("User not found.", false);
            }

            return user;
        });
    }
}