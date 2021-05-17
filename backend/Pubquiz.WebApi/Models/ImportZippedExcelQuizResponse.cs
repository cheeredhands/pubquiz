using System.Collections.Generic;
using Pubquiz.Domain.Models;

namespace Pubquiz.WebApi.Models
{
    public class ImportZippedExcelQuizResponse : ApiResponse
    {
        public List<QuizRef> QuizRefs { get; set; }
    }
}