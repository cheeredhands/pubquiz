using System.Collections.Generic;
using System.Linq;

namespace Pubquiz.Domain.Models
{
    public class InteractionResponse
    {
        public int InteractionId { get; set; }
        public List<int> ChoiceOptionIds { get; set; }
        public string Response { get; set; }

        public bool FlaggedForManualCorrection { get; set; }
        public bool ManuallyCorrected { get; set; }
        public bool ManualCorrectionOutcome { get; set; }
        public int AwardedScore { get; set; }

        public InteractionResponse()
        {
        }

        public InteractionResponse(int interactionId)
        {
            InteractionId = interactionId;
        }

        public InteractionResponse(int interactionId, IEnumerable<int> choiceOptionIds, string response = "") : this(
            interactionId)
        {
            ChoiceOptionIds = choiceOptionIds.ToList();
            Response = response;
        }

        public InteractionResponse(int interactionId, string response) : this(interactionId)
        {
            Response = response;
        }

        public void Correct(bool outcome)
        {
            ManualCorrectionOutcome = outcome;
            ManuallyCorrected = true;
        }
    }
}