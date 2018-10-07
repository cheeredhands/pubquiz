using System;
using Pubquiz.Persistence;

namespace Pubquiz.Domain.Models
{
    public class User : Model
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RecoveryCode { get; set; }
        public UserRole UserRole { get; set; }
    }

    public enum UserRole
    {
        Team,
        Admin,
        QuizMaster
    }
}