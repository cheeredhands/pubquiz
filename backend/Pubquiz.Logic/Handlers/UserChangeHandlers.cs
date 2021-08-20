using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Messages;
using Pubquiz.Logic.Requests.Commands;
using Pubquiz.Logic.Requests.Notifications;
using Pubquiz.Logic.Requests.Queries;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Handlers
{
    public class UserChangeHandlers : Handler, IRequestHandler<LogoutUserCommand>, IRequestHandler<SelectGameCommand>,
        IRequestHandler<LoginCommand, User>
    {
        public UserChangeHandlers(IUnitOfWork unitOfWork, IMediator mediator, ILoggerFactory loggerFactory) : base(
            unitOfWork, mediator, loggerFactory)
        {
        }

        public async Task<Unit> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
        {
            var userCollection = UnitOfWork.GetCollection<User>();
            var user = await userCollection.GetAsync(request.UserId);
            if (user == null)
            {
                throw new DomainException(ResultCode.InvalidEntityId, "Invalid User id", false);
            }

            await Mediator.Publish(new UserLoggedOut(user.Id, user.UserName, user.CurrentGameId), cancellationToken);
            return Unit.Value;
        }

        public async Task<Unit> Handle(SelectGameCommand request, CancellationToken cancellationToken)
        {
            var userCollection = UnitOfWork.GetCollection<User>();
            var user = await userCollection.GetAsync(request.ActorId);
            if (user.UserRole != UserRole.QuizMaster)
            {
                throw new DomainException(ResultCode.UnauthorizedRole, "You can't do that with this role.", true);
            }

            if (!user.GameIds.Contains(request.GameId))
            {
                throw new DomainException(ResultCode.QuizMasterUnauthorizedForGame,
                    $"Actor with id {request.ActorId} is not authorized for game '{request.GameId}'", true);
            }

            var oldGameId = user.CurrentGameId;
            user.CurrentGameId = request.GameId;
            await userCollection.UpdateAsync(user);

            var query = new QmLobbyViewModelQuery
            {
                UserId = request.ActorId
            };
            var viewModel = await Mediator.Send(query, cancellationToken);
            await Mediator.Publish(new GameSelected(request.ActorId, oldGameId, request.GameId, viewModel),
                cancellationToken);
            return Unit.Value;
        }

        public async Task<User> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var userCollection = UnitOfWork.GetCollection<User>();
            var user = await userCollection.FirstOrDefaultAsync(u =>
                u.UserName == request.UserName && u.Password == request.Password);

            if (user == null)
            {
                throw new DomainException(ResultCode.InvalidUserNameOrPassword, "Invalid username or password.", true);
            }

            return user;
        }
    }
}