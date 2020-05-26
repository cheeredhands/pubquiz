using System;
using System.Collections.Generic;
using System.Linq;
using Pubquiz.Persistence;

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
                            interactionResponse.FlaggedForManualCorrection = false;
                        }
                        else
                        {
                            // todo levenshtein/soundex whatever checks
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

        public Interaction()
        {
        }
    }

    public class Solution
    {
        public List<int> ChoiceOptionIds { get; set; }
        public List<string> Responses { get; set; }
        public int LevenshteinTolerance { get; set; } = 2; // default levenshtein tolerance
        public bool FlagIfWithinTolerance { get; set; }

        public Solution()
        {
        }

        public Solution(IEnumerable<int> optionIds)
        {
            ChoiceOptionIds = optionIds.ToList();
        }

        public Solution(IEnumerable<string> responses, int levenshteinTolerance = 2,
            bool flagIfWithinTolerance = false)
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

        public ChoiceOption()
        {
        }
    }

    public enum InteractionType
    {
        MultipleChoice,
        MultipleResponse,
        ShortAnswer,
        ExtendedText
    }

    public class MediaObject
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Uri { get; }
        public Dimensions Dimensions { get; set; }
        public MediaType MediaType { get; }

        public MediaObject(string uri, MediaType mediaType)
        {
            Id = Guid.NewGuid();
            Uri = uri;
            MediaType = mediaType;
        }

        public MediaObject()
        {
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

    /// <summary>
    /// Contains approximate string matching
    /// https://www.dotnetperls.com/levenshtein
    /// </summary>
    public static class LevenshteinDistance
    {
        /// <summary>
        /// Compute the distance between two strings.
        /// </summary>
        public static int Compute(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            // Step 7
            return d[n, m];
        }
    }
}