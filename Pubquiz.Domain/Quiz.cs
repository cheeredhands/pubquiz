using System;
using System.Collections.Generic;

namespace Pubquiz.Domain
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

    public class QuestionSet
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public List<Question> Questions { get; set; }

        public QuestionSet()
        {
            Id = Guid.NewGuid();
            Questions = new List<Question>();
        }
    }
}