using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Messages;
using Pubquiz.Persistence;
using Rebus.Bus;
using Rebus.Handlers;

namespace Pubquiz.Logic.Handlers
{
    public class ScoringHandler : IHandleMessages<InteractionResponseAdded>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBus _bus;

        public ScoringHandler(IUnitOfWork unitOfWork, IBus bus)
        {
            _unitOfWork = unitOfWork;
            _bus = bus;
        }

        public async Task Handle(InteractionResponseAdded message)
        {
            // score it
            var teamCollection = _unitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(message.TeamId);
            if (team == null)
            {
                // log it somewhere, or send a message (with a DomainException?) to the hub so the quizmaster knows something went wrong?
                // something like:
                var exception = new DomainException(ResultCode.InvalidTeamId, "Team could not be found while scoring answer.", true);
                await _bus.Publish(new ErrorOccurred(exception));
                return;
            }

            var answer = team.Answers.FirstOrDefault(a => a.QuizItemId == message.QuizItemId);
            if (answer == null)
            {
                // log it somewhere, or send a message (with a DomainException?) to the hub so the quizmaster knows something went wrong?
                return;
            }

            var questionCollection = _unitOfWork.GetCollection<QuizItem>();
            var question = await questionCollection.GetAsync(message.QuizItemId);
            if (question == null)
            {
                // log it somewhere, or send a message (with a DomainException?) to the hub so the quizmaster knows something went wrong?
                return;
            }

            // score it!
            question.Score(answer);
            team.UpdateScore();
            
            // send AnswerScored message, so the clients will be notified and the team scores and dashboard will be updated
            await _bus.Publish(new AnswerScored
            {
                TeamId = team.Id,
                GameId = message.GameId,
                QuizItemId = message.QuizItemId,
                InteractionId = message.InteractionId,
                Response = message.Response,
                QuizItemScore = answer.TotalScore,
                TotalTeamScore = team.TotalScore,
                InteractionResponses = answer.InteractionResponses
            });
        }
    }
}