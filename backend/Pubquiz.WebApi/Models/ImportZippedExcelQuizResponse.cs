using System.Collections.Generic;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.ViewModels;

namespace Pubquiz.WebApi.Models
{
    public class ImportZippedExcelQuizResponse : ApiResponse
    {
        public List<QmQuizViewModel> QuizViewModels { get; set; }
    }
}