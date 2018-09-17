using System;
using System.Collections.Generic;
using System.Text;

namespace Pubquiz.Domain.Models
{
   public abstract class QuizItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public List<Media> Media { get; set; }
    }
}
