using System;
using System.Collections.Generic;
using Pubquiz.Persistence;

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
        public List<QuizSection> QuizSections { get; set; }

        public Quiz()
        {
            Id = Guid.NewGuid();
            QuizSections = new List<QuizSection>();
        }
    }
}