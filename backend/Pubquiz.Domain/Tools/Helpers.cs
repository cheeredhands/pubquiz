using System;
using System.Collections.Generic;
using Pubquiz.Domain.Models;
using Pubquiz.Repository;

namespace Pubquiz.Domain.Tools
{
    public static class Helpers
    {
        private static readonly List<string> Wordlist = new List<string>
        {
            "flatulent", "stroef", "extra", "dof", "peuzel", "aap", "noot", "mies", "wim", "zus", "jet", "teun", "vuur",
            "gijs", "lam", "kees", "weide", "does", "hok", "duif", "schapen"
        };

        public static string GenerateSessionRecoveryCode(IRepository<Team> teamRepository, Guid gameId)
        {
            string result;
            do
            {
                var words = new List<string>();
                var random = new Random();
                var wordlistLength = Wordlist.Count;
                for (int i = 0; i < 3; i++)
                {
                    var nextIndex = random.Next(0, wordlistLength);
                    words.Add(Wordlist[nextIndex]);
                }

                result = string.Join(" ", words);
            } while (teamRepository.AnyAsync(t => t.RecoveryCode == result && t.GameId == gameId).Result);

            return result;
        }
    }
}