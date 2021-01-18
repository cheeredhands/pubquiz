using System.Threading.Tasks;
using MediatR;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Messages;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Requests.Notifications
{
    public class LogoutUserNotification : Notification
    {
        public string UserId { get; set; }

        public LogoutUserNotification(IUnitOfWork unitOfWork, IMediator mediator) : base(unitOfWork, mediator)
        {
        }

        protected override async Task DoExecute()
        {
            var userCollection = UnitOfWork.GetCollection<User>();
            var user = await userCollection.GetAsync(UserId);
            if (user == null)
            {
                throw new DomainException(ResultCode.InvalidUserId, "Invalid User id", false);
            }
            
            await Mediator.Publish(new UserLoggedOut(user.Id, user.UserName, user.CurrentGameId));
        }
    }
}