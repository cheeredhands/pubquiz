using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Pubquiz.Domain.Models
{
    public class Answer
    {
        public string QuizSectionId { get; set; }
        public string QuizItemId { get; set; }
        public List<InteractionResponse> InteractionResponses { get; set; }
        public int TotalScore { get; set; }
        public bool FlaggedForManualCorrection { get; set; }

        public Answer(string quizSectionId, string quizItemId)
        {
            QuizSectionId = quizSectionId;
            QuizItemId = quizItemId;
            InteractionResponses = new List<InteractionResponse>();
        }

        public Answer()
        {
        }

        public void CorrectInteraction(int interactionId, bool correct)
        {
            var interactionResponse = InteractionResponses.FirstOrDefault(r => r.InteractionId == interactionId);
            if (interactionResponse == null)
            {
                interactionResponse = new InteractionResponse(interactionId);
                interactionResponse.Correct(correct);
                InteractionResponses.Add(interactionResponse);
            }
            else
            {
                interactionResponse.Correct(correct);
            }
        }

        public void SetInteractionResponse(int interactionId, IEnumerable<int> choiceOptionIds, string response)
        {
            var interactionResponse = InteractionResponses.FirstOrDefault(r => r.InteractionId == interactionId);
            if (interactionResponse == null)
            {
                if (choiceOptionIds == null)
                {
                    interactionResponse = new InteractionResponse(interactionId, response);
                }
                else
                {
                    interactionResponse = new InteractionResponse(interactionId, choiceOptionIds, response);
                }
                
                InteractionResponses.Add(interactionResponse);
            }
            else
            {
                interactionResponse.ChoiceOptionIds =
                    choiceOptionIds == null ? new List<int>() : choiceOptionIds.ToList();
                interactionResponse.Response = response;
            }
            
            // reset the manual correction flags
            interactionResponse.ManuallyCorrected = false;
            interactionResponse.FlaggedForManualCorrection = false;
        }
    }
}