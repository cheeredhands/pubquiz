using System;
using System.Collections.Generic;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Pubquiz.Domain.Models
{
    public class Team
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<string> MemberNames { get; set; }
        public int TotalScore { get; set; }
        public Dictionary<Guid, int> ScorePerQuestionSet { get; set; }

        public List<Answer> Answers { get; set; }

        public Team()
        {
            Id = Guid.NewGuid();
        }
    }
}