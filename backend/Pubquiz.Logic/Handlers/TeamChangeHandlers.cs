using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Messages;
using Pubquiz.Logic.Requests.Notifications;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Handlers
{
    public class TeamChangeHandlers : Handler, INotificationHandler<ChangeTeamMembersNotification>,
        INotificationHandler<ChangeTeamNameNotification>, INotificationHandler<DeleteTeamNotification>,
        INotificationHandler<LogoutTeamNotification>
    {
        private readonly ILogger<TeamChangeHandlers> _logger;

        public TeamChangeHandlers(IUnitOfWork unitOfWork, IMediator mediator, ILoggerFactory loggerFactory) : base(
            unitOfWork, mediator, loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TeamChangeHandlers>();
        }

        private async Task CheckTeamId(string teamId)
        {
            var entityCollection = UnitOfWork.GetCollection<Team>();
            var entity = await entityCollection.GetAsync(teamId);
            if (entity == null)
            {
                throw new DomainException(ResultCode.InvalidTeamId, $"Invalid TeamId.", true);
            }
        }

        public async Task Handle(ChangeTeamMembersNotification notification, CancellationToken cancellationToken)
        {
            await CheckTeamId(notification.TeamId);
            if (notification.TeamMembers.Length > ValidationValues.MaxTeamMembersLength)
            {
                throw new DomainException(ResultCode.ValidationError,
                    $"Team members maximum length is {ValidationValues.MaxTeamMembersLength} characters.", true);
            }

            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(notification.TeamId);

            notification.TeamMembers = SanitizeTeamMembers(notification.TeamMembers);
            team.MemberNames = notification.TeamMembers;

            await teamCollection.UpdateAsync(team);
            await Mediator.Publish(
                new TeamMembersChanged(team.CurrentGameId, notification.TeamId, team.Name, notification.TeamMembers),
                cancellationToken);
        }


        private string SanitizeTeamMembers(string teamMembers)
        {
            // replace multiple whitespace by just one
            var result = Regex.Replace(teamMembers, @"(\s){2,}", "$1");

            // trim whitespace
            return result.Trim();
        }

        public async Task Handle(ChangeTeamNameNotification notification, CancellationToken cancellationToken)
        {
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(notification.TeamId);

            // check if team name is taken, otherwise throw DomainException
            var isTeamNameTaken = teamCollection.AsQueryable().ToList().Any(t =>
                string.Equals(t.Name, notification.NewName, StringComparison.CurrentCultureIgnoreCase) &&
                t.CurrentGameId == team.CurrentGameId);
            if (isTeamNameTaken)
            {
                throw new DomainException(ResultCode.TeamNameIsTaken, "Team name is taken.", true);
            }

            // set new name
            var oldTeamName = team.Name;
            team.Name = notification.NewName.Trim();
            team.UserName = notification.NewName.Trim();
            await teamCollection.UpdateAsync(team);
            await Mediator.Publish(
                new TeamNameUpdated(notification.TeamId, team.CurrentGameId, oldTeamName, notification.NewName),
                cancellationToken);
        }

        public async Task Handle(DeleteTeamNotification notification, CancellationToken cancellationToken)
        {
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(notification.TeamId);
            var user = await UnitOfWork.GetCollection<User>().GetAsync(notification.ActorId);

            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(team.CurrentGameId);
            if (user.UserRole != UserRole.Admin)
            {
                if (game.QuizMasterIds.All(i => i != notification.ActorId))
                {
                    throw new DomainException(ResultCode.QuizMasterUnauthorizedForGame,
                        $"Actor with id {notification.ActorId} is not authorized for game '{game.Id}'", true);
                }
            }

            game.TeamIds.Remove(team.Id);
            await gameCollection.UpdateAsync(game);
            await teamCollection.DeleteAsync(notification.TeamId);

            await Mediator.Publish(new TeamDeleted(notification.TeamId, game.Id), cancellationToken);
        }

        public async Task Handle(LogoutTeamNotification notification, CancellationToken cancellationToken)
        {
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(notification.TeamId);
            if (team == null)
            {
                _logger.LogInformation("Unknown team (probably old session with inmemory db), proceed with logout.");
                return;
            }

            await Mediator.Publish(new TeamLoggedOut(team.Id, team.Name, team.CurrentGameId), cancellationToken);
        }
    }
}