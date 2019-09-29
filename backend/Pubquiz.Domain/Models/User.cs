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
        public List<string> GameIds { get; set; }
        public string CurrentGameId { get; set; }
        public bool IsLoggedIn { get; set; }

        public User()
        {
            GameIds = new List<string>();
        }
    }

    public enum UserRole
    {
        Team,
        Admin,
        QuizMaster
    }
}