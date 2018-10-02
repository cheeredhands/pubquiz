using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.Requests;
using Pubquiz.Domain.Tools;
using Pubquiz.Persistence;


namespace Pubquiz.WebApi.Helpers
{
    public class MyUserStore : IUserStore<ApplicationUser>
    {
        private readonly IUnitOfWork _unitOfWork;

        public MyUserStore(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Dispose()
        {
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.Id.ToString());

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.UserName);


        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
            return new Task(async () =>
            {
                var userRepo = _unitOfWork.GetCollection<User>();
                var teamRepo = _unitOfWork.GetCollection<Team>();
                var userUser = await userRepo.GetAsync(user.Id);
                var teamUser = await teamRepo.GetAsync(user.Id);
                userUser.UserName = userName;
                teamUser.UserName = userName;
                await userRepo.UpdateAsync(userUser);
                await teamRepo.UpdateAsync(teamUser);
            }, cancellationToken);
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.NormalizedUserName);

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName,
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
            return new Task(async () =>
            {
                var userRepo = _unitOfWork.GetCollection<User>();
                var teamRepo = _unitOfWork.GetCollection<Team>();
                var userUser = await userRepo.GetAsync(user.Id);
                var teamUser = await teamRepo.GetAsync(user.Id);
                userUser.NormalizedUserName = normalizedName;
                teamUser.NormalizedUserName = normalizedName;
                await userRepo.UpdateAsync(userUser);
                await teamRepo.UpdateAsync(teamUser);
            }, cancellationToken);
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var command = new CreateUserCommand(_unitOfWork)
            {
                User = new User
                {
                    UserName = user.UserName,
                    NormalizedUserName = user.NormalizedUserName,
                    RecoveryCode = user.Code
                }
            };
            try
            {
                var newUser = await command.Execute();
                user.Id = newUser.Id;
            }
            catch (DomainException exception)
            {
                _unitOfWork.Abort();
                return IdentityResult.Failed(new IdentityError
                    {Code = "Domain error", Description = exception.Message});
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Only team names can be updated for now
            var command = new ChangeTeamNameNotification(_unitOfWork)
                {TeamId = user.Id, NewName = user.UserName};
            try
            {
                await command.Execute();
            }
            catch (DomainException exception)
            {
                return IdentityResult.Failed(new IdentityError
                    {Code = "Domain error", Description = exception.Message});
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var command = new DeleteTeamNotification(_unitOfWork) {TeamId = user.Id};
            try
            {
                await command.Execute();
            }
            catch (DomainException exception)
            {
                return IdentityResult.Failed(new IdentityError
                    {Code = "Domain error", Description = exception.Message});
            }

            return IdentityResult.Success;
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!Guid.TryParse(userId, out var guidId))
            {
                throw new ArgumentException($"{userId} is not a valid guid.");
            }

            var query = new GetUserByIdQuery(_unitOfWork) {UserId = guidId};
            try
            {
                var user = await query.Execute();
                return ApplicationUser.FromUser(user);
            }
            catch (DomainException)
            {
                return null;
            }
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = new GetUserByNormalizedUserNameQuery(_unitOfWork)
                {NormalizedUserName = normalizedUserName};

            try
            {
                var user = await query.Execute();
                return ApplicationUser.FromUser(user);
            }
            catch (DomainException)
            {
                return null;
            }
        }
    }
}