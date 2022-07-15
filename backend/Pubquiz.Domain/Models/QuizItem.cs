using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Pubquiz.Domain.Models
{
    public class QuizItem : Model
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public List<MediaObject> MediaObjects { get; set; }

        public QuizItemType QuizItemType { get; set; }
        public int MaxScore { get; set; }
        public List<Interaction> Interactions { get; set; }

        public QuizItem()
        {
            MediaObjects = new List<MediaObject>();
            Interactions = new List<Interaction>();
        }

        public void Score(Answer answer)
        {
            foreach (var interactionResponse in answer.InteractionResponses)
            {
                var interaction = Interactions.First(i => i.Id == interactionResponse.InteractionId);

                var solution = interaction.Solution;
                switch (interaction.InteractionType)
                {
                    case InteractionType.MultipleChoice:
                    case InteractionType.MultipleResponse:
                        ScoreMultipleChoice(interactionResponse, interaction, solution);
                        break;
                    case InteractionType.ShortAnswer:
                        ScoreShortAnswer(interactionResponse, interaction, solution);
                        break;
                    case InteractionType.ExtendedText:
                        ScoreExtendedText(interactionResponse, interaction);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            answer.TotalScore = answer.InteractionResponses.Sum(i => i.AwardedScore);

            answer.FlaggedForManualCorrection =
                answer.InteractionResponses.Any(i => i.FlaggedForManualCorrection && !i.ManuallyCorrected);
        }

        private static void ScoreExtendedText(InteractionResponse interactionResponse, Interaction interaction)
        {
            if (interactionResponse.ManuallyCorrected)
            {
                interactionResponse.AwardedScore =
                    interactionResponse.ManualCorrectionOutcome ? interaction.MaxScore : 0;
                interactionResponse.FlaggedForManualCorrection = false;
                return;
            }

            interactionResponse.FlaggedForManualCorrection = true;
        }

        private static void ScoreShortAnswer(InteractionResponse interactionResponse, Interaction interaction,
            Solution solution)
        {
            if (interactionResponse.ManuallyCorrected)
            {
                interactionResponse.AwardedScore =
                    interactionResponse.ManualCorrectionOutcome ? interaction.MaxScore : 0;
                interactionResponse.FlaggedForManualCorrection = false;
                return;
            }

            if (solution.Responses.Contains(interactionResponse.Response))
            {
                interactionResponse.AwardedScore = interaction.MaxScore;
                interactionResponse.FlaggedForManualCorrection = false;
            }
            else
            {
                // levenshtein checks
                var smallestLevenshteinDistance = solution.Responses.Min(s =>
                    LevenshteinDistance.Compute(s.ToLowerInvariant(), interactionResponse.Response.ToLowerInvariant()));
                if (smallestLevenshteinDistance <= solution.LevenshteinTolerance)
                {
                    interactionResponse.AwardedScore = interaction.MaxScore;
                    if (solution.FlagIfWithinTolerance)
                    {
                        interactionResponse.FlaggedForManualCorrection = true;
                    }
                }
                else
                {
                    interactionResponse.AwardedScore = 0;
                    interactionResponse.FlaggedForManualCorrection = true;
                }
            }
        }

        private static void ScoreMultipleChoice(InteractionResponse interactionResponse, Interaction interaction,
            Solution solution)
        {
            if (interactionResponse.ManuallyCorrected)
            {
                interactionResponse.AwardedScore =
                    interactionResponse.ManualCorrectionOutcome ? interaction.MaxScore : 0;
                interactionResponse.FlaggedForManualCorrection = false;
                return;
            }

            var correctOptionIds = solution.ChoiceOptionIds;
            var responseOptionIds = interactionResponse.ChoiceOptionIds;
            if (correctOptionIds.Count == responseOptionIds.Count &&
                correctOptionIds.All(responseOptionIds.Contains))
            {
                interactionResponse.AwardedScore = interaction.MaxScore;
            }
            else
            {
                interactionResponse.AwardedScore = 0;
            }
        }
    }
}