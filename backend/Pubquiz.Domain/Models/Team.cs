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
        public Dictionary<Guid, int> ScorePerQuestionSet { get; set; }

        public Guid GameId { get; set; }

        public List<Answer> Answers { get; set; }

        public Team()
        {
            Id = Guid.NewGuid();
        }
    }
}