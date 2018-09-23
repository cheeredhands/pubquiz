using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.Requests;
using Pubquiz.Domain.Tools;
using Pubquiz.Repository;


namespace Pubquiz.WebApi.Helpers
{
    public class UserStore : IUserStore<IdentityUser>
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public UserStore(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public void Dispose()
        {
        }

        public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.Id.ToString());

        public Task<string> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.UserName);


        public Task SetUserNameAsync(IdentityUser user, string userName, CancellationToken cancellationToken)
        {
            return new Task(async () =>
            {
                var userRepo = _repositoryFactory.GetRepository<User>();
                var teamRepo = _repositoryFactory.GetRepository<Team>();
                var userUser = await userRepo.GetAsync(user.Id);
                var teamUser = await teamRepo.GetAsync(user.Id);
                userUser.UserName = userName;
                teamUser.UserName = userName;
                await userRepo.UpdateAsync(userUser);
                await teamRepo.UpdateAsync(teamUser);
            }, cancellationToken);
        }

        public Task<string> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.NormalizedUserName);

        public Task SetNormalizedUserNameAsync(IdentityUser user, string normalizedName,
            CancellationToken cancellationToken)
        {
            return new Task(async () =>
            {
                var userRepo = _repositoryFactory.GetRepository<User>();
                var teamRepo = _repositoryFactory.GetRepository<Team>();
                var userUser = await userRepo.GetAsync(user.Id);
                var teamUser = await teamRepo.GetAsync(user.Id);
                userUser.NormalizedUserName = normalizedName;
                teamUser.NormalizedUserName = normalizedName;
                await userRepo.UpdateAsync(userUser);
                await teamRepo.UpdateAsync(teamUser);
            }, cancellationToken);
        }

        public async Task<IdentityResult> CreateAsync(IdentityUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var command = new RegisterForGameCommand(_repositoryFactory)
                {TeamName = user.UserName, Code = user.Code};
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

        public async Task<IdentityResult> UpdateAsync(IdentityUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Only team names can be updated for now
            var command = new ChangeTeamNameNotification(_repositoryFactory)
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

        public async Task<IdentityResult> DeleteAsync(IdentityUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var command = new DeleteTeamNotification(_repositoryFactory) {TeamId = user.Id};
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

        public async Task<IdentityUser> FindByIdAsync(string userId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!Guid.TryParse(userId, out var guidId))
            {
                throw new ArgumentException($"{userId} is not a valid guid.");
            }

            var query = new GetUserByIdQuery(_repositoryFactory) {UserId = guidId};
            var user = await query.Execute();
            return IdentityUser.FromUser(user);
        }

        public async Task<IdentityUser> FindByNameAsync(string normalizedUserName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = new GetUserByNormalizedUserNameQuery(_repositoryFactory)
                {NormalizedUserName = normalizedUserName};
            var user = await query.Execute();
            return IdentityUser.FromUser(user);
        }
    }
}