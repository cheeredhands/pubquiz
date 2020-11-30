using System;
using System.Collections.Generic;
using Pubquiz.Persistence;

namespace Pubquiz.Domain.Models
{
    public class User : Model
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RecoveryCode { get; set; }
        public UserRole UserRole { get; set; }
        public List<GameRef> GameRefs { get; set; }
        public string CurrentGameId { get; set; }
        public bool IsLoggedIn => ConnectionCount > 0;
        public int ConnectionCount { get; set; }

        public User()
        {
            GameRefs = new List<GameRef>();
        }
    }

    public enum UserRole
    {
        Team,
        Admin,
        QuizMaster
    }
}