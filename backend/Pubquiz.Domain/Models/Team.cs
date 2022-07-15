using System.Collections.Generic;
using System.Linq;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Pubquiz.Domain.Models
{
    public class Team : User
    {
        public string Name { get; set; }
        public string MemberNames { get; set; }
        public int TotalScore { get; set; }
        public Dictionary<string, int> ScorePerQuizSection { get; set; }

        public Dictionary<string, Answer> Answers { get; set; }

        public Team()
        {
            Answers = new Dictionary<string, Answer>();
            ScorePerQuizSection = new Dictionary<string, int>();
        }

        public void UpdateScore()
        {
            ScorePerQuizSection = new Dictionary<string, int>();

            foreach (var answer in Answers.Values)
            {
                if (!ScorePerQuizSection.ContainsKey(answer.QuizSectionId))
                {
                    ScorePerQuizSection[answer.QuizSectionId] = 0;
                }
                ScorePerQuizSection[answer.QuizSectionId] += answer.TotalScore;
            }

            TotalScore = ScorePerQuizSection.Values.Sum();
        }
    }
}