using System.Collections.Generic;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.ViewModels;

namespace Pubquiz.WebApi.Models
{
    public class LoginResponse : ApiResponse
    {
        public string Jwt { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string CurrentGameId { get; set; }
    }
}