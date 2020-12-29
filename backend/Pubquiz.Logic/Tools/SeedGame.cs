using System;
using System.Collections.Generic;
using System.Linq;
using Pubquiz.Domain.Models;
using Pubquiz.Persistence.Extensions;

namespace Pubquiz.Logic.Tools
{
    public static class SeedGame
    {
        public static Game GetGame(IEnumerable<string> quizMasterIds, Quiz quiz)
        {
            var game = new Game
            {
                Id = Guid.Parse("5C4ED8FB-005A-4521-9678-3CB20374FBDF").ToShortGuidString(),
                QuizId = quiz.Id,
                Title = "PéCé",
                QuizTitle = quiz.Title,
                InviteCode = "PECE",
                QuizMasterIds = quizMasterIds.ToList(),
                TotalQuestionCount = quiz.TotalQuestionCount,
                TotalQuizItemCount = quiz.TotalQuizItemCount,
                CurrentSectionQuizItemCount = quiz.QuizSections[0].QuizItemRefs.Count,
                CurrentSectionIndex = 1,
                CurrentSectionId = quiz.QuizSections[0].Id,
                CurrentQuizItemId = quiz.QuizSections[0].QuizItemRefs[0].Id,
                CurrentQuizItemIndexInSection = 1,
                CurrentQuizItemIndexInTotal = 1,
                CurrentQuestionIndexInTotal = 0,
                State = GameState.Open
            };

            return game;
        }
    }
}