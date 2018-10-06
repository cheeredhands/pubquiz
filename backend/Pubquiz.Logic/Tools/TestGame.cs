using Pubquiz.Domain.Models;

namespace Pubquiz.Logic.Tools
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