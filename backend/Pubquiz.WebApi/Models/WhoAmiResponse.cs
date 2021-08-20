using System.Collections.Generic;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.ViewModels;

namespace Pubquiz.WebApi.Models
{
    public class WhoAmiResponse : ApiResponse
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string MemberNames { get; set; }
        public string CurrentGameId { get; set; }
        public GameState GameState { get; set; }
        public UserRole UserRole { get; set; }
        public string RecoveryCode { get; set; }
    }
}