using System.Threading.Tasks;
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


            // send AnswerScored message

            await _bus.Publish(new AnswerScored());
        }
    }
}