using System;
using System.Collections.Generic;
using System.Linq;
using Pubquiz.Domain.ViewModels;
using Pubquiz.Persistence;
using Pubquiz.Persistence.Extensions;

// ReSharper disable CollectionNeverUpdated.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Pubquiz.Domain.Models
{
    /// <summary>
    /// A planned instance of a game that uses a certain quiz.
    /// Changes in time throughout the game, e.g. has state.
    /// </summary>
    public class Game : Model
    {
        public string Title { get; set; }
        public GameState State { get; set; }
        public string QuizId { get; set; }
        public List<string> TeamIds { get; set; }
        public List<string> QuizMasterIds { get; set; }
        public string InviteCode { get; set; }

        public Question CurrentQuestion { get; set; }
        public int CurrentQuestionIndex { get; set; }
        public string CurrentQuestionId { get; set; }
        public int CurrentQuizSectionIndex { get; set; }
        public string CurrentQuizSectionId { get; set; }

        public Game()
        {
            State = GameState.Closed;
            QuizId = Guid.Empty.ToShortGuidString();
            TeamIds = new List<string>();
        }

        public void SetState(GameState newGameState)
        {
            switch (newGameState)
            {
                case GameState.Closed:
                    if (State != GameState.Open)
                    {
                        throw new DomainException(ErrorCodes.InvalidGameStateTransition,
                            "Can only close the game from the open state.", true);
                    }

                    break;
                case GameState.Open:
                    if (State != GameState.Closed)
                    {
                        throw new DomainException(ErrorCodes.InvalidGameStateTransition,
                            "Can only open the game from the closed state.", true);
                    }

                    if (State == GameState.Closed && (QuizId == Guid.Empty.ToShortGuidString() || string.IsNullOrWhiteSpace(Title)))
                    {
                        throw new DomainException(ErrorCodes.InvalidGameStateTransition,
                            "Can't open the game without a quiz and/or a title.", true);
                    }

                    break;
                case GameState.Running:
                    if (State != GameState.Open && State != GameState.Paused)
                    {
                        throw new DomainException(ErrorCodes.InvalidGameStateTransition,
                            "Can only start the game from the open and paused states.", true);
                    }

                    if (!TeamIds.Any())
                    {
                        throw new DomainException(ErrorCodes.InvalidGameStateTransition,
                            "Can't start the game without teams.", true);
                    }

                    break;
                case GameState.Paused:
                    if (State != GameState.Running)
                    {
                        throw new DomainException(ErrorCodes.InvalidGameStateTransition,
                            "Can only pause the game from the running state.", true);
                    }

                    break;

                case GameState.Finished:
                    if (State != GameState.Running && State != GameState.Paused)
                    {
                        throw new DomainException(ErrorCodes.InvalidGameStateTransition,
                            "Can only finish the game from the running and paused states.", true);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newGameState), newGameState, null);
            }

            State = newGameState;
        }

        public GameViewModel ToViewModel()
        {
            return new GameViewModel
            {
                GameId = Id,
                State = State,
                GameTitle = Title
            };
        }
    }
}