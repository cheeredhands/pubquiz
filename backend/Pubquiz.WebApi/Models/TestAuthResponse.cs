using System.Collections.Generic;
using Pubquiz.Domain.Models;

namespace Pubquiz.WebApi.Models
{
    public class TestAuthResponse : ApiResponse
    {
        public List<Team> Teams { get; set; }
    }
}