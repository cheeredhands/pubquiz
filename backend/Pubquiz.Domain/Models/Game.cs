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
        public string QuizTitle { get; set; }
        public GameState State { get; set; }
        public string QuizId { get; set; }
        public List<string> TeamIds { get; set; }
        public List<string> QuizMasterIds { get; set; }
        public string InviteCode { get; set; }

        public int TotalQuizItemCount { get; set; }
        public int TotalQuestionCount { get; set; }
        public int CurrentSectionQuizItemCount { get; set; }

        /// <summary>
        /// 1-based index.
        /// </summary>
        public int CurrentSectionIndex { get; set; }

        public string CurrentSectionId { get; set; }
        public string CurrentSectionTitle { get; set; }
        public string CurrentQuizItemId { get; set; }

        /// <summary>
        /// 1-based index.
        /// </summary>
        public int CurrentQuizItemIndexInSection { get; set; }

        /// <summary>
        /// 1-based index.
        /// </summary>
        public int CurrentQuizItemIndexInTotal { get; set; }
        /// <summary>
        /// 1-based index.
        /// </summary>
        public int CurrentQuestionIndexInTotal { get; set; }

        public Game()
        {
            State = GameState.Closed;
            QuizId = Guid.Empty.ToShortGuidString();
            TeamIds = new List<string>();
        }

        public QmGameViewModel ToQmGameViewModel() {
            return new()
            {
                Id = Id,
                Title = Title,
                InviteCode = InviteCode,
                QuizId = QuizId,
                QuizTitle = QuizTitle,
                GameState = State
            };
        }

        public void SetState(GameState newGameState)
        {
            // switch (newGameState)
            // {
            //     case GameState.Closed:
            //         if (State != GameState.Open)
            //         {
            //             throw new DomainException(ResultCode.InvalidGameStateTransition,
            //                 "Can only close the game from the open state.", true);
            //         }
            //
            //         break;
            //     case GameState.Open:
            //         if (State != GameState.Closed)
            //         {
            //             throw new DomainException(ResultCode.InvalidGameStateTransition,
            //                 "Can only open the game from the closed state.", true);
            //         }
            //
            //         if (State == GameState.Closed &&
            //             (QuizId == Guid.Empty.ToShortGuidString() || string.IsNullOrWhiteSpace(Title)))
            //         {
            //             throw new DomainException(ResultCode.InvalidGameStateTransition,
            //                 "Can't open the game without a quiz and/or a title.", true);
            //         }
            //
            //         break;
            //     case GameState.Running:
            //         if (State != GameState.Open && State != GameState.Paused)
            //         {
            //             throw new DomainException(ResultCode.InvalidGameStateTransition,
            //                 "Can only start the game from the open and paused states.", true);
            //         }
            //
            //         if (!TeamIds.Any())
            //         {
            //             throw new DomainException(ResultCode.InvalidGameStateTransition,
            //                 "Can't start the game without teams.", true);
            //         }
            //
            //         break;
            //     case GameState.Paused:
            //         if (State != GameState.Running)
            //         {
            //             throw new DomainException(ResultCode.InvalidGameStateTransition,
            //                 "Can only pause the game from the running state.", true);
            //         }
            //
            //         break;
            //
            //     case GameState.Finished:
            //         if (State != GameState.Running && State != GameState.Paused)
            //         {
            //             throw new DomainException(ResultCode.InvalidGameStateTransition,
            //                 "Can only finish the game from the running and paused states.", true);
            //         }
            //
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException(nameof(newGameState), newGameState, null);
            // }

            State = newGameState;
        }
    }
}