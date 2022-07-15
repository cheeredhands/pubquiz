using System.Collections.Generic;

namespace Pubquiz.Domain.Models
{
    public class User : Model
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RecoveryCode { get; set; }
        public UserRole UserRole { get; set; }
        public List<string> QuizIds { get; set; } = new();
        public List<string> GameIds { get; set; } = new();
        public string CurrentGameId { get; set; }
        public bool IsLoggedIn => ConnectionCount > 0;
        public int ConnectionCount { get; set; }
    }

    public enum UserRole
    {
        Team,
        Admin,
        QuizMaster
    }
}