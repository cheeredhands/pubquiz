using System;
using System.Collections.Generic;
using Pubquiz.Persistence;

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
        public Dictionary<Guid, int> ScorePerQuizSection { get; set; }

        public Guid GameId { get; set; }

        public List<Answer> Answers { get; set; }

        public Team()
        {
            Id = Guid.NewGuid();
            Answers = new List<Answer>();
            ScorePerQuizSection = new Dictionary<Guid, int>();
        }

        public void UpdateScore()
        {
            ScorePerQuizSection = new Dictionary<Guid, int>();

            foreach (var answer in Answers)
            {
                ScorePerQuizSection[answer.QuizSectionId] += answer.TotalScore;
            }
        }
    }
}