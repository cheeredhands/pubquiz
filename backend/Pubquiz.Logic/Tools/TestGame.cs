using System;
using System.Collections.Generic;
using System.Linq;
using Pubquiz.Domain.Models;

namespace Pubquiz.Logic.Tools
{
    public static class TestGame
    {
        public static Game GetGame(IEnumerable<Guid> quizMasterIds)
        {
            var game = new Game
            {
                Title = "Test game",
                InviteCode = "JOINME",
                QuizMasterIds = quizMasterIds.ToList()
            };

            return game;
        }
    }
}