using System;
using System.Collections.Generic;
using System.Linq;
using Pubquiz.Domain.Tools;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Pubquiz.Domain.Models
{
    public class Answer
    {
        public Guid QuizSectionId { get; set; }
        public Guid QuestionId { get; set; }
        public List<InteractionResponse> InteractionResponses { get; set; }
        public int TotalScore { get; set; }
        public bool FlaggedForManualCorrection { get; set; }

        public Answer(Guid quizSectionId, Guid questionId)
        {
            QuizSectionId = quizSectionId;
            QuestionId = questionId;
            InteractionResponses = new List<InteractionResponse>();
        }

        public void Score(Question question)
        {
            foreach (var interactionResponse in InteractionResponses)
            {
                var interaction = question.Interactions.First(i => i.Id == interactionResponse.InteractionId);

                var solution = interaction.Solution;
                switch (interaction.InteractionType)
                {
                    case InteractionType.MultipleChoice:
                    case InteractionType.MultipleResponse:
                        var correctOptionIds = solution.ChoiceOptionIds;
                        var responseOptionIds = interactionResponse.ChoiceOptionIds;
                        if (correctOptionIds.Count == responseOptionIds.Count &&
                            correctOptionIds.All(responseOptionIds.Contains))
                        {
                            interactionResponse.AwardedScore = interaction.MaxScore;
                            //TotalScore += interaction.MaxScore;
                        }

                        break;
                    case InteractionType.ShortAnswer:
                        if (interactionResponse.ManuallyCorrected)
                        {
                            interactionResponse.AwardedScore =
                                interactionResponse.ManualCorrectionOutcome ? interaction.MaxScore : 0;
                            break;
                        }

                        if (solution.Responses.Contains(interactionResponse.Response))
                        {
                            interactionResponse.AwardedScore = interaction.MaxScore;
                        }
                        else
                        {
                            // todo levenshtein/soundex whatever checks
                            interactionResponse.FlaggedForManualCorrection = true;
                        }

                        break;
                    case InteractionType.ExtendedText:
                        if (interactionResponse.ManuallyCorrected)
                        {
                            interactionResponse.AwardedScore =
                                interactionResponse.ManualCorrectionOutcome ? interaction.MaxScore : 0;
                            break;
                        }

                        interactionResponse.FlaggedForManualCorrection = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            TotalScore = InteractionResponses.Sum(i => i.AwardedScore);

            FlaggedForManualCorrection =
                InteractionResponses.Any(i => i.FlaggedForManualCorrection && !i.ManuallyCorrected);
        }
    }

    public class InteractionResponse
    {
        public int InteractionId { get; set; }
        public List<int> ChoiceOptionIds { get; set; }
        public string Response { get; set; }

        public bool FlaggedForManualCorrection { get; set; }
        public bool ManuallyCorrected { get; set; }
        public bool ManualCorrectionOutcome { get; set; }
        public int AwardedScore { get; set; }

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