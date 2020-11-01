using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Messages;
using Pubquiz.Persistence;
using Rebus.Bus;
using Rebus.Handlers;

namespace Pubquiz.Logic.Handlers
{
    public class ScoringHandler : IHandleMessages<InteractionResponseAdded>, IHandleMessages<InteractionCorrected>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBus _bus;
        private readonly ILogger<ScoringHandler> _logger;

        public ScoringHandler(IUnitOfWork unitOfWork, IBus bus, ILoggerFactory loggerFactory)
        {
            _unitOfWork = unitOfWork;
            _bus = bus;
            _logger = loggerFactory.CreateLogger<ScoringHandler>();
        }

        public async Task Handle(InteractionResponseAdded message)
        {
            _logger.LogInformation($"Start scoring an answer.");
            // score it
            var teamCollection = _unitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(message.TeamId);
            if (team == null)
            {
                _logger.LogInformation($"Scoring: Team is null.");
                // something like:
                var exception = new DomainException(ResultCode.InvalidTeamId, "Team could not be found while scoring answer.", true);
                await _bus.Publish(new ErrorOccurred(exception));
                return;
            }
            
            team.Answers.TryGetValue(message.QuizItemId, out var answer);
            if (answer == null)
            {
                _logger.LogInformation($"Scoring: Answer is null.");
                return;
            }

            var quizItemCollection = _unitOfWork.GetCollection<QuizItem>();
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
            
            await _bus.Publish(new AnswerScored
            {
                TeamId = team.Id,
                GameId = message.GameId,
                QuizItemId = message.QuizItemId,
                InteractionId = message.InteractionId,
                TotalTeamScore = team.TotalScore,
                ScorePerQuizSection = team.ScorePerQuizSection,
                Answer = answer
            });
        }

        public async Task Handle(InteractionCorrected message)
        {
            _logger.LogInformation($"Start scoring an answer.");
            // score it
            var teamCollection = _unitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(message.TeamId);
            if (team == null)
            {
                _logger.LogInformation($"Scoring: Team is null.");
                // something like:
                var exception = new DomainException(ResultCode.InvalidTeamId, "Team could not be found while scoring answer.", true);
                await _bus.Publish(new ErrorOccurred(exception));
                return;
            }
            
            team.Answers.TryGetValue(message.QuizItemId, out var answer);
            if (answer == null)
            {
                _logger.LogInformation($"Scoring: Answer is null.");
                return;
            }

            var quizItemCollection = _unitOfWork.GetCollection<QuizItem>();
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
            
            await _bus.Publish(new AnswerScored
            {
                TeamId = team.Id,
                GameId = message.GameId,
                QuizItemId = message.QuizItemId,
                InteractionId = message.InteractionId,
                TotalTeamScore = team.TotalScore,
                ScorePerQuizSection = team.ScorePerQuizSection,
                Answer = answer
            });
        }
    }
}