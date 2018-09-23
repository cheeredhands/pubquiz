using System;
using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.Tools;
using Pubquiz.Repository;

namespace Pubquiz.Domain.Requests
{
    public class GetUserByIdQuery : Query<User>
    {
        public Guid UserId { get; set; }

        public GetUserByIdQuery(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        protected override async Task<User> DoExecute()
        {
            var userRepo = RepositoryFactory.GetRepository<User>();

            var user = await userRepo.GetAsync(UserId);
            if (user == null)
            {
                throw new DomainException("User not found.", false);
            }

            return user;
        }
    }

    public class GetUserByNormalizedUserNameQuery : Query<User>
    {
        public string NormalizedUserName { get; set; }

        public GetUserByNormalizedUserNameQuery(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        protected override Task<User> DoExecute() => Task.Run(() =>
        {
            var userRepo = RepositoryFactory.GetRepository<User>();

            var user = userRepo.AsQueryable().FirstOrDefault(u => u.NormalizedUserName == NormalizedUserName);
            if (user == null)
            {
                throw new DomainException("User not found.", false);
            }

            return user;
        });
    }
}