using System;
using System.Text;
using Pubquiz.Persistence;

namespace Pubquiz.Domain.Models
{
    public class QuizItemRef
    {
        public string Id { get; set; }
        public QuizItemType ItemType { get; set; }
        public string Title { get; set; }
        public QuizItemRef(QuizItem quizItem)
        {
            Id = quizItem.Id;
            ItemType = quizItem.QuizItemType;
            Title = quizItem.Title;
        }

        public QuizItemRef()
        {
        }
    }
}