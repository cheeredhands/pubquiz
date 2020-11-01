using System.Collections.Generic;
using System.Linq;

namespace Pubquiz.Domain.Models
{
    public class Solution
    {
        public List<int> ChoiceOptionIds { get; set; }
        public List<string> Responses { get; set; }
        public int LevenshteinTolerance { get; set; } = 2; // default levenshtein tolerance
        public bool FlagIfWithinTolerance { get; set; }

        public Solution()
        {
        }

        public Solution(IEnumerable<int> optionIds)
        {
            ChoiceOptionIds = optionIds.ToList();
        }

        public Solution(IEnumerable<string> responses, int levenshteinTolerance = 2,
            bool flagIfWithinTolerance = false)
        {
            Responses = responses.ToList();
            LevenshteinTolerance = levenshteinTolerance;
            FlagIfWithinTolerance = flagIfWithinTolerance;
        }
    }
}