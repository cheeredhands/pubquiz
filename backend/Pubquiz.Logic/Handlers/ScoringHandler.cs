using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Messages;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Handlers
{
    public class ScoringHandler : Handler, INotificationHandler<InteractionResponseAdded>, INotificationHandler<InteractionCorrected>
    {
        private readonly ILogger<ScoringHandler> _logger;

        public ScoringHandler(IUnitOfWork unitOfWork, IMediator mediator, ILoggerFactory loggerFactory) : base(unitOfWork, mediator, loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ScoringHandler>();
        }

        public async Task Handle(InteractionResponseAdded message, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Start scoring an answer.");
            // score it
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(message.TeamId);
            if (team == null)
            {
                _logger.LogInformation($"Scoring: Team is null.");
                // something like:
                var exception = new DomainException(ResultCode.InvalidTeamId, "Team could not be found while scoring answer.", true);
                await Mediator.Publish(new ErrorOccurred(exception), cancellationToken);
                return;
            }
            
            team.Answers.TryGetValue(message.QuizItemId, out var answer);
            if (answer == null)
            {
                _logger.LogInformation($"Scoring: Answer is null.");
                return;
            }

            var quizItemCollection = UnitOfWork.GetCollection<QuizItem>();
            var quizItem = await quizItemCollection.GetAsync(message.QuizItemId);
            if (quizItem == null)
            {
                _logger.LogInformation($"Scoring: QuizItem is null.");
                return;
            }

            // score it!
            quizItem.Score(answer);
            _logger.LogDebug($"scored", quizItem.Interactions);
            team.UpdateScore();

            await teamCollection.UpdateAsync(team);
            
            _logger.LogDebug($"Done scoring an answer.");
            
            await Mediator.Publish(new AnswerScored
            {
                TeamId = team.Id,
                GameId = message.GameId,
                QuizItemId = message.QuizItemId,
                InteractionId = message.InteractionId,
                TotalTeamScore = team.TotalScore,
                ScorePerQuizSection = team.ScorePerQuizSection,
                Answer = answer
            }, cancellationToken);
        }

        public async Task Handle(InteractionCorrected message, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Start scoring an answer.");
            // score it
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(message.TeamId);
            if (team == null)
            {
                _logger.LogInformation($"Scoring: Team is null.");
                // something like:
                var exception = new DomainException(ResultCode.InvalidTeamId, "Team could not be found while scoring answer.", true);
                await Mediator.Publish(new ErrorOccurred(exception), cancellationToken);
                return;
            }
            
            team.Answers.TryGetValue(message.QuizItemId, out var answer);
            if (answer == null)
            {
                _logger.LogInformation($"Scoring: Answer is null.");
                return;
            }

            var quizItemCollection = UnitOfWork.GetCollection<QuizItem>();
            var quizItem = await quizItemCollection.GetAsync(message.QuizItemId);
            if (quizItem == null)
            {
                _logger.LogInformation($"Scoring: QuizItem is null.");
                return;
            }

            // score it!
            quizItem.Score(answer);
            team.UpdateScore();

            await teamCollection.UpdateAsync(team);
            
            _logger.LogInformation($"Done scoring an answer.");
            
            await Mediator.Publish(new AnswerScored
            {
                TeamId = team.Id,
                GameId = message.GameId,
                QuizItemId = message.QuizItemId,
                InteractionId = message.InteractionId,
                TotalTeamScore = team.TotalScore,
                ScorePerQuizSection = team.ScorePerQuizSection,
                Answer = answer
            }, cancellationToken);
        }
    }
}