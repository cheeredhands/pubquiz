using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Messages;
using Pubquiz.Logic.Requests.Commands;
using Pubquiz.Logic.Requests.Notifications;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Handlers
{
    public class TeamChangeHandlers : Handler, IRequestHandler<ChangeTeamMembersCommand>,
        IRequestHandler<ChangeTeamNameCommand>, IRequestHandler<DeleteTeamCommand>,
        IRequestHandler<LogoutTeamCommand>, IRequestHandler<CorrectInteractionCommand>,
        IRequestHandler<SubmitInteractionResponseCommand>, IRequestHandler<RegisterForGameCommand, Team>
    {
        private readonly ILogger<TeamChangeHandlers> _logger;

        public TeamChangeHandlers(IUnitOfWork unitOfWork, IMediator mediator, ILoggerFactory loggerFactory) : base(
            unitOfWork, mediator, loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TeamChangeHandlers>();
        }

        public async Task<Unit> Handle(ChangeTeamMembersCommand notification, CancellationToken cancellationToken)
        {
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
            return Unit.Value;
        }

        private string SanitizeTeamMembers(string teamMembers)
        {
            // replace multiple whitespace by just one
            var result = Regex.Replace(teamMembers, @"(\s){2,}", "$1");

            // trim whitespace
            return result.Trim();
        }

        public async Task<Unit> Handle(ChangeTeamNameCommand command, CancellationToken cancellationToken)
        {
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(command.TeamId);

            // check if team name is taken, otherwise throw DomainException
            var isTeamNameTaken = teamCollection.AsQueryable().ToList().Any(t =>
                string.Equals(t.Name, command.NewName, StringComparison.CurrentCultureIgnoreCase) &&
                t.CurrentGameId == team.CurrentGameId);
            if (isTeamNameTaken)
            {
                throw new DomainException(ResultCode.TeamNameIsTaken, "Team name is taken.", true);
            }

            // set new name
            var oldTeamName = team.Name;
            team.Name = command.NewName.Trim();
            team.UserName = command.NewName.Trim();
            await teamCollection.UpdateAsync(team);
            await Mediator.Publish(
                new TeamNameUpdated(command.TeamId, team.CurrentGameId, oldTeamName, command.NewName),
                cancellationToken);
            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteTeamCommand command, CancellationToken cancellationToken)
        {
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(command.TeamId);
            var user = await UnitOfWork.GetCollection<User>().GetAsync(command.ActorId);

            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(team.CurrentGameId);
            if (user.UserRole != UserRole.Admin)
            {
                if (!game.QuizMasterIds.Contains(command.ActorId))
                {
                    throw new DomainException(ResultCode.QuizMasterUnauthorizedForGame,
                        $"Actor with id {command.ActorId} is not authorized for game '{game.Id}'", true);
                }
            }

            game.TeamIds.Remove(team.Id);
            await gameCollection.UpdateAsync(game);
            await teamCollection.DeleteAsync(command.TeamId);

            await Mediator.Publish(new TeamDeleted(command.TeamId, game.Id), cancellationToken);
            return Unit.Value;
        }

        public async Task<Unit> Handle(LogoutTeamCommand command, CancellationToken cancellationToken)
        {
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(command.TeamId);
            if (team == null)
            {
                _logger.LogInformation("Unknown team (probably old session with inmemory db), proceed with logout");
                return Unit.Value;
            }

            await Mediator.Publish(new TeamLoggedOut(team.Id, team.Name, team.CurrentGameId), cancellationToken);
            return Unit.Value;
        }

        public async Task<Unit> Handle(CorrectInteractionCommand request, CancellationToken cancellationToken)
        {
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(request.TeamId);
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(team.CurrentGameId);
            var quizId = game.QuizId;

            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            var quiz = await quizCollection.GetAsync(quizId);

            var quizSectionId = quiz.QuizSections
                .FirstOrDefault(qs => qs.QuestionItemRefs.Any(q => q.Id == request.QuizItemId))?.Id;
            if (string.IsNullOrWhiteSpace(quizSectionId))
            {
                throw new DomainException(ResultCode.QuestionNotInQuiz, "This question doesn't belong to the quiz.",
                    true);
            }

            // save response
            team.Answers.TryGetValue(request.QuizItemId, out var answer);
            if (answer == null)
            {
                answer = new Answer(quizSectionId, request.QuizItemId);
                team.Answers.Add(request.QuizItemId, answer);
            }

            answer.CorrectInteraction(request.InteractionId, request.Correct);

            await teamCollection.UpdateAsync(team);
            await Mediator.Publish(
                new InteractionCorrected(game.Id, request.TeamId, request.QuizItemId, request.InteractionId,
                    request.Correct), cancellationToken);
            return Unit.Value;
        }

        public async Task<Unit> Handle(SubmitInteractionResponseCommand request, CancellationToken cancellationToken)
        {
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(request.TeamId);
            var quizItemCollection = UnitOfWork.GetCollection<QuizItem>();
            var quizItem = await quizItemCollection.GetAsync(request.QuizItemId);
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(team.CurrentGameId);
            var quizId = game.QuizId;

            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            var quiz = await quizCollection.GetAsync(quizId);

            var quizSectionId = quiz.QuizSections
                .FirstOrDefault(qs => qs.QuestionItemRefs.Any(q => q.Id == request.QuizItemId))?.Id;
            if (string.IsNullOrWhiteSpace(quizSectionId))
            {
                throw new DomainException(ResultCode.QuestionNotInQuiz, "This question doesn't belong to the quiz.",
                    true);
            }

            if (game.CurrentSectionId != quizSectionId)
            {
                throw new DomainException(ResultCode.QuestionNotInCurrentQuizSection,
                    "This question doesn't belong to the current quiz section.", true);
            }

            if (game.State == GameState.Paused || game.State == GameState.Finished)
            {
                throw new DomainException(ResultCode.GameIsPausedOrFinished,
                    "The game is paused or finished. Submitting answers is not allowed.", true);
            }

            if (quizItem.Interactions.All(i => i.Id != request.InteractionId))
            {
                throw new DomainException(ResultCode.InvalidEntityId, "Invalid InteractionId.", true);
            }

            // save response
            team.Answers.TryGetValue(request.QuizItemId, out var answer);
            if (answer == null)
            {
                answer = new Answer(quizSectionId, request.QuizItemId);
                team.Answers.Add(request.QuizItemId, answer);
            }

            answer.SetInteractionResponse(request.InteractionId, request.ChoiceOptionIds, request.Response);

            await teamCollection.UpdateAsync(team);

            var response = string.IsNullOrWhiteSpace(request.Response)
                ? GetChoiceOptionTexts(quizItem, request.InteractionId, request.ChoiceOptionIds)
                : request.Response;
            await Mediator.Publish(new InteractionResponseAdded(game.Id, request.TeamId, request.QuizItemId,
                request.InteractionId, response), cancellationToken);
            return Unit.Value;
        }

        private string GetChoiceOptionTexts(QuizItem question, int interactionId, List<int> choiceOptionIds)
        {
            if (choiceOptionIds == null)
            {
                return string.Empty;
            }

            var choiceOptionTexts = new List<string>();
            foreach (var choiceOptionId in choiceOptionIds)
            {
                choiceOptionTexts.Add(question.Interactions[interactionId].ChoiceOptions[choiceOptionId].Text);
            }

            return string.Join(", ", choiceOptionTexts);
        }

        public async Task<Team> Handle(RegisterForGameCommand request, CancellationToken cancellationToken)
        {
            var gameCollection = UnitOfWork.GetCollection<Game>();

            // check validity of invite code, otherwise throw DomainException
            var game = gameCollection.AsQueryable().FirstOrDefault(g => g.InviteCode == request.Code);

            if (game != null && (game.State == GameState.Closed || game.State == GameState.Finished))
            {
                throw new DomainException(ResultCode.InvalidCode, "Invalid or expired code.", true);
            }

            var teamCollection = UnitOfWork.GetCollection<Team>();
            Team team = null;
            if (game == null)
            {
                // check if it's a recovery code for a team
                team = teamCollection.AsQueryable().FirstOrDefault(t => t.RecoveryCode == request.Code);
                if (team == null)
                {
                    throw new DomainException(ResultCode.InvalidCode, "Invalid code.", false);
                }
            }

            if (team == null)
            {
                // check if team name is taken, otherwise throw DomainException
                var isTeamNameTaken = !string.IsNullOrWhiteSpace(request.Name) &&
                                      teamCollection.AsQueryable().ToList().Any(t =>
                                          string.Equals(t.Name, request.Name,
                                              StringComparison.InvariantCultureIgnoreCase) &&
                                          t.CurrentGameId == game.Id);
                if (isTeamNameTaken)
                {
                    throw new DomainException(ResultCode.TeamNameIsTaken, "Team name is taken.", true);
                }

                // register team and return team object
                var userName = request.Name?.Trim();
                var recoveryCode = Helpers.GenerateSessionRecoveryCode(teamCollection, game.Id);

                team = new Team
                {
                    Name = userName,
                    UserName = userName,
                    CurrentGameId = game.Id,
                    RecoveryCode = recoveryCode,
                    UserRole = UserRole.Team
                };
                var user = new User
                {
                    Id = team.Id,
                    UserName = userName,
                    CurrentGameId = game.Id,
                    RecoveryCode = recoveryCode,
                    UserRole = UserRole.Team
                };

                game.TeamIds.Add(team.Id);
                await teamCollection.AddAsync(team);
                var userCollection = UnitOfWork.GetCollection<User>();
                await userCollection.AddAsync(user);
                await gameCollection.UpdateAsync(game);
            }
            else
            {
                team.Name = string.IsNullOrWhiteSpace(request.Name) ? team.Name : request.Name;
                await teamCollection.UpdateAsync(team);
            }

            await Mediator.Publish(new TeamRegistered(team.Id, team.Name, team.CurrentGameId, team.MemberNames),
                cancellationToken);
            await Mediator.Publish(new QmTeamRegistered {GameId = team.CurrentGameId, Team = team}, cancellationToken);
            return team;
        }
    }
}