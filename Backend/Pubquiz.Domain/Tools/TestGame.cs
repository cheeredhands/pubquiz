using System.Collections.Generic;
using System.Linq;
using Pubquiz.Domain.Models;
using Pubquiz.Repository;

namespace Pubquiz.Domain.Tools
{
    public static class TestGame
    {
        public static Game GetGame()
        {
            var game = new Game
            {
                Title = "Test game",
                InviteCode = "JOINME"
                };            

            return game;
        }
    }
}