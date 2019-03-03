using System;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Messages;
using Pubquiz.Persistence;
using Rebus.Bus;

namespace Pubquiz.Logic.Requests
{
    public class LogoutUserNotification : Notification
    {
        public Guid UserId { get; set; }

        public LogoutUserNotification(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork, bus)
        {
        }

        protected override async Task DoExecute()
        {
            var userCollection = UnitOfWork.GetCollection<User>();
            var user = await userCollection.GetAsync(UserId);
            if (user == null)
            {
                throw new DomainException(ErrorCodes.InvalidUserId, "Invalid User id", false);
            }

            user.IsLoggedIn = false;

            await userCollection.UpdateAsync(user);
            
            await Bus.Publish(new UserLoggedOut(user.Id, user.UserName, user.CurrentGameId));
        }
    }
}