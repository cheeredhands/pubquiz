using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Persistence;
using Rebus.Bus;

namespace Pubquiz.Logic.Requests
{
    public class SelectGameCommand : Command<User>
    {
        public Guid ActorId { get; set; }
        public Guid GameId { get; set; }

        public SelectGameCommand(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork, bus)
        {
        }

        protected override async Task<User> DoExecute()
        {
            var userCollection = UnitOfWork.GetCollection<User>();
            var user = await userCollection.GetAsync(ActorId);
            if (user == null)
            {
                throw new DomainException(ErrorCodes.InvalidUserId, "Invalid user id.", true);
            }

            if (user.UserRole != UserRole.QuizMaster)
            {
                throw new DomainException(ErrorCodes.UnauthorizedRole, "You can't do that with this role.", true);
            }

            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(GameId);
            if (game == null)
            {
                throw new DomainException(ErrorCodes.InvalidGameId, "Invalid game id.", true);
            }

            if (!user.GameIds.Contains(GameId))
            {
                throw new DomainException(ErrorCodes.QuizMasterUnauthorizedForGame,
                    $"Actor with id {ActorId} is not authorized for game '{GameId}'", true);
            }

            return user;
        }
    }
}