using System.Collections.Generic;
using Pubquiz.Persistence;

namespace Pubquiz.Domain.Models
{
    public class QuizrPackage : Model
    {
        public string Hash { get; set; }
        public string FullPath { get; set; }
        public string Filename { get; set; }
        public string PackageMetadata { get; set; }
        public List<string> QuizIds { get; set; }

        public QuizrPackage()
        {
            QuizIds = new List<string>();
        }
    }
}