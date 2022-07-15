using System.Collections.Generic;
using Pubquiz.Domain.ViewModels;

namespace Pubquiz.Domain.Models
{
    public class QuizrPackage : Model
    {
        public string Hash { get; set; }
        public string FullPath { get; set; }
        public string Filename { get; set; }
        public string PackageMetadata { get; set; }
        public List<QmQuizViewModel> QuizViewModels { get; set; } = new();
    }
}