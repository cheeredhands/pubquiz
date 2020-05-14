using System;
using System.Collections.Generic;
using System.Linq;
using Pubquiz.Domain.Models;
using Pubquiz.Persistence.Extensions;

namespace Pubquiz.Logic.Tools
{
    public static class TestGame
    {
        public static Game GetGame(IEnumerable<string> quizMasterIds, Quiz quiz)
        {
            var game = new Game
            {
                Id = Guid.Parse("B193D5B8-F9B4-448B-B1C6-D96DEF188B51").ToShortGuidString(),
                QuizId = quiz.Id,
                Title = $"Test game with quiz '{quiz.Title}'",
                InviteCode = "JOINME",
                QuizMasterIds = quizMasterIds.ToList(),
                TotalQuestionCount = quiz.TotalQuestionCount,
                TotalQuizItemCount = quiz.TotalQuizItemCount,
                CurrentSectionQuizItemCount = quiz.QuizSections[1].QuizItems.Count,
                CurrentSectionIndex = 2,
                CurrentSectionId = quiz.QuizSections[1].Id,
                CurrentQuizItemId = quiz.QuizSections[1].QuizItems[0].Id,
                CurrentQuizItemIndexInSection = 1,
                CurrentQuizItemIndexInTotal = 2,
                CurrentQuestionIndexInTotal = 1,
                State = GameState.Open
            };

            return game;
        }
    }
}