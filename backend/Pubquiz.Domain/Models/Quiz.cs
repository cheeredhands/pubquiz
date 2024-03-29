using System.Collections.Generic;
using System.Linq;
using Pubquiz.Domain.ViewModels;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Pubquiz.Domain.Models
{
    /// <summary>
    /// An instance of a quiz (a composition of questions) that can be held at some time.
    /// Contains question sets which contain questions.
    /// </summary>
    public class Quiz : Model
    {
        public string Title { get; set; }
        public List<QuizSection> QuizSections { get; set; } = new();
        public int TotalQuizItemCount => QuizSections.Sum(qs => qs.QuizItemRefs.Count);
        public int TotalQuestionCount => QuizSections.Sum(qs => qs.QuestionItemRefs.Count);
        public IEnumerable<string> QuizItemIds => QuizSections.SelectMany(qs => qs.QuizItemRefs.Select(qi => qi.Id));

        public QmQuizViewModel ToQmQuizViewModel()
        {
            return new QmQuizViewModel
            {
                Id = Id,
                Title = Title
            };
        }
    }
}