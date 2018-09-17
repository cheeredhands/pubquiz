using System;
using System.Collections.Generic;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Pubquiz.Domain.Models
{
    public class Quiz
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<QuestionSet> QuestionSets { get; set; }

        public Quiz()
        {
            Id = Guid.NewGuid();
            QuestionSets = new List<QuestionSet>();
        }
    }
}