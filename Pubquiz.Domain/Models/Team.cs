using System;
using System.Collections.Generic;
using Citolab.Repository;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Pubquiz.Domain.Models
{
    public class Team : Model
    {
        public string Name { get; set; }
        public List<string> MemberNames { get; set; }
        public int TotalScore { get; set; }
        public Dictionary<Guid, int> ScorePerQuestionSet { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }

        public List<Answer> Answers { get; set; }

        public Team()
        {
            Id = Guid.NewGuid();
        }
    }
}