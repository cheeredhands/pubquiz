using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Pubquiz.Domain.Models
{
    /// <summary>
    /// A section of a quiz, e.g. a round or a category.
    /// </summary>
    public class QuizSection
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<QuizItem> QuizItems { get; set; }

        /// <summary>
        /// Only the questions in this section.
        /// </summary>
        public List<QuizItem> QuestionItems => QuizItems.Where(q => q.ItemType == ItemType.Question).ToList();

        public QuizSection()
        {
            Id = Guid.NewGuid();
            QuizItems = new List<QuizItem>();
        }
    }
}