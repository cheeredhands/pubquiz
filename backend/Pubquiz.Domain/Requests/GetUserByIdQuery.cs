using System;
using System.Threading.Tasks;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Domain.Requests
{
    public class GetUserByIdQuery : Query<User>
    {
        public Guid UserId { get; set; }

        public GetUserByIdQuery(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override async Task<User> DoExecute()
        {
            var userRepo = UnitOfWork.GetCollection<User>();

            var user = await userRepo.GetAsync(UserId);
            if (user == null)
            {
                throw new DomainException("User not found.", false);
            }

            return user;
        }
    }
}