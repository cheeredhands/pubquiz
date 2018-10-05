using System;
using System.Collections.Generic;
using System.Text;
using Pubquiz.Persistence;

namespace Pubquiz.Domain.Models
{
    public class QuizItem : Model
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public List<Media> Media { get; set; }
    }
}