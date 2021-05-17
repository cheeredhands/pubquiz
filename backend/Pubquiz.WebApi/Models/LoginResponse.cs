using System.Collections.Generic;
using Pubquiz.Domain.Models;

namespace Pubquiz.WebApi.Models
{
    public class LoginResponse : ApiResponse
    {
        public string Jwt { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string CurrentGameId { get; set; }
        public List<QuizRef> QuizRefs { get; set; }
        public List<GameRef> GameRefs { get; set; }
    }
}