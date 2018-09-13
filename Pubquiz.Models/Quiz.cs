using System;
using System.Collections.Generic;

namespace Pubquiz.Models
{
    public class Quiz
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<QuestionSet> QuestionSets { get; set; }
    }
    
    public class QuestionSet
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public List<Question> Questions { get; set; }
    }
}