using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Pubquiz.Domain.Models;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Requests
{
    public class UserQuery : Query<User>
    {
        public Guid UserId { get; set; }

        public UserQuery(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override async Task<User> DoExecute()
        {
            var userCollection = UnitOfWork.GetCollection<User>();
            var user = await userCollection.GetAsync(UserId);
            return user;
        }
    }
}