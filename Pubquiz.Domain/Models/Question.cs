using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Pubquiz.Domain.Models
{
    public class Question
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public List<Media> Media { get; set; }
        public QuestionType QuestionType { get; set; }
        public int MaxScore { get; set; }
        public List<Interaction> Interactions { get; set; }

        public Question()
        {
            Id = Guid.NewGuid();
            Media = new List<Media>();
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
                    case InteractionType.MC:
                    case InteractionType.MR:
                        var correctOptionIds = solution.ChoiceOptionIds;
                        var responseOptionIds = interactionResponse.ChoiceOptionIds;
                        if (correctOptionIds.Count == responseOptionIds.Count &&
                            correctOptionIds.All(responseOptionIds.Contains))
                        {
                            interactionResponse.AwardedScore = interaction.MaxScore;
                            //TotalScore += interaction.MaxScore;
                        }

                        break;
                    case InteractionType.SA:
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
                    case InteractionType.ET:
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

            answer.TotalScore = answer.InteractionResponses.Sum(i => i.AwardedScore);

            answer.FlaggedForManualCorrection =
                answer.InteractionResponses.Any(i => i.FlaggedForManualCorrection && !i.ManuallyCorrected);
        }
    }

    public class Interaction
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int MaxScore { get; set; }
        public List<ChoiceOption> ChoiceOptions { get; set; }
        public InteractionType InteractionType { get; set; }
        public Solution Solution { get; set; }

        public Interaction(int id)
        {
            Id = id;
            ChoiceOptions = new List<ChoiceOption>();
        }
    }

    public class Solution
    {
        public List<int> ChoiceOptionIds { get; set; }
        public List<string> Responses { get; set; }
        public int LevenshteinTolerance { get; set; }
        public bool FlagIfWithinTolerance { get; set; }

        public Solution(IEnumerable<int> optionIds)
        {
            ChoiceOptionIds = optionIds.ToList();
        }

        public Solution(IEnumerable<string> responses, int levenshteinTolerance = 0, bool flagIfWithinTolerance = false)
        {
            Responses = responses.ToList();
            LevenshteinTolerance = levenshteinTolerance;
            FlagIfWithinTolerance = flagIfWithinTolerance;
        }
    }

    public class ChoiceOption
    {
        public int Id { get; set; }

        public string Text { get; set; }
        // maybe later? public Media Media { get; set; }

        public ChoiceOption(int id, string text)
        {
            Id = id;
            Text = text;
        }
    }

    public enum QuestionType
    {
        /// <summary>
        /// Multiple choice
        /// </summary>
        MC,

        /// <summary>
        /// Multiple response
        /// </summary>
        MR,

        /// <summary>
        /// Short answer
        /// </summary>
        SA,

        /// <summary>
        /// Extended text
        /// </summary>
        ET,

        /// <summary>
        /// Mixed (multiple interaction at the question level)
        /// </summary>
        Mixed
    }

    public enum InteractionType
    {
        /// <summary>
        /// Multiple choice
        /// </summary>
        MC,

        /// <summary>
        /// Multiple response
        /// </summary>
        MR,

        /// <summary>
        /// Short answer
        /// </summary>
        SA,

        /// <summary>
        /// Extended text
        /// </summary>
        ET
    }

    public class Media
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Uri { get; }
        public Dimensions Dimensions { get; set; }
        public MediaType MediaType { get; }

        public Media(string uri, MediaType mediaType)
        {
            Id = Guid.NewGuid();
            Uri = uri;
            MediaType = mediaType;
        }
    }

    public enum MediaType
    {
        Image,
        Video,
        Audio
    }

    public class Dimensions
    {
        public int OriginalWidth { get; set; }
        public int OriginalHeight { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int DurationInSeconds { get; set; }
    }
}