using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Pubquiz.Models
{
    public class Game
    {
        public Guid Id { get; set; }
        public GameState State { get; set; }
        public Quiz Quiz { get; set; }
        public List<Team> Teams { get; set; }

        public Question CurrentQuestion { get; set; }
        public int CurrentQuestionIndex { get; set; }
        public int CurrentQuestionSetIndex { get; set; }
    }

    public enum GameState
    {
        Open, // e.g. open for registration
        Started, // rename? InSession? Running?
        Paused,
        Closed
    }

    public class Team
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<string> MemberNames { get; set; }
        public int TotalScore { get; set; }
        public Dictionary<Guid, int> ScorePerQuestionSet { get; set; }

        public List<Answer> Answers { get; set; }
    }

    public class Answer
    {
        public Guid QuestionSetId { get; set; }
        public Guid QuestionId { get; set; }
        public List<InteractionResponse> InteractionResponses { get; set; }
        public int TotalScore { get; set; }
        public bool FlaggedForManualCorrection { get; set; }

        public Answer()
        {
            InteractionResponses = new List<InteractionResponse>();
        }

        public void Score()
        {
            foreach (var interactionResponse in InteractionResponses)
            {
                // todo get interaction
                var interaction = new Interaction {Id = interactionResponse.InteractionId};
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
        public Guid InteractionId { get; set; }
        public List<Guid> ChoiceOptionIds { get; set; }
        public string Response { get; set; }

        public bool FlaggedForManualCorrection { get; set; }
        public bool ManuallyCorrected { get; set; }
        public bool ManualCorrectionOutcome { get; set; }
        public int AwardedScore { get; set; }

        public void Correct(bool outcome)
        {
            ManualCorrectionOutcome = outcome;
            ManuallyCorrected = true;
        }
    }
}