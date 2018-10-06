using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Requests
{
    public class CreateUserCommand : Command<User>
    {
        public User User { get; set; }

        public CreateUserCommand(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override async Task<User> DoExecute()
        {
//            var teams = UnitOfWork.GetCollection<Team>();
//            var users = UnitOfWork.GetCollection<User>();
//
//            // check if team exists
//            var team = await teams.GetAsync(User.Id);
//            if (team == null)
//            {
//                throw new DomainException($"Team with id {User.Id} doesn't exist.", true);
//            }
//
//            throw new System.NotImplementedException();

            var users = UnitOfWork.GetCollection<User>();
            var existingUser = users.FirstOrDefaultAsync(u => u.UserName == User.UserName).Result;
            if (existingUser != null)
            {
                throw new DomainException($"User with username {User.UserName} already exists.", true);
            }

            User = await users.AddAsync(User);
            return User;
        }
    }
}