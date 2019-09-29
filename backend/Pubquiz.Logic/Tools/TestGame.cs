using System;
using System.Collections.Generic;
using System.Linq;
using Pubquiz.Domain.Models;
using Pubquiz.Persistence.Extensions;

namespace Pubquiz.Logic.Tools
{
    public static class TestGame
    {
        public static Game GetGame(IEnumerable<string> quizMasterIds, int currentQuizSectionIndex,
            string currentQuizSectionId)
        {
            var game = new Game
            {
                Id = Guid.Parse("B193D5B8-F9B4-448B-B1C6-D96DEF188B51").ToShortGuidString(),
                Title = "Test game",
                InviteCode = "JOINME",
                QuizMasterIds = quizMasterIds.ToList(),
                CurrentQuizSectionIndex = currentQuizSectionIndex,
                CurrentQuizSectionId = currentQuizSectionId,
                State = GameState.Open
            };

            return game;
        }
    }
}