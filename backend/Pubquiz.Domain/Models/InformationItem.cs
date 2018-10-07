using System.Collections.Generic;
using Pubquiz.Persistence;

namespace Pubquiz.Domain.Models
{
    /// <summary>
    /// An informational quiz item, so not a question.
    /// Can be used as a divider between rounds or as a header
    /// and footer of the quiz.
    /// </summary>
    public class InformationItem : Model
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public List<Media> Media { get; set; }
        
        public InformationItem()
        {
           
        }
    }
}