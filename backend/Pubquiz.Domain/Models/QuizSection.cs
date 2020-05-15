using System;
using System.Collections.Generic;
using System.Linq;
using Pubquiz.Persistence.Extensions;

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
        public string Id { get; set; }
        public string Title { get; set; }
        public List<QuizItemRef> QuizItemRefs { get; set; }

        /// <summary>
        /// Only the questions in this section.
        /// </summary>
        public List<QuizItemRef> QuestionItemRefs => QuizItemRefs.Where(q => q.ItemType != QuizItemType.Information).ToList();

        public QuizSection()
        {
            Id = Guid.NewGuid().ToShortGuidString();
            QuizItemRefs = new List<QuizItemRef>();
        }
    }
}