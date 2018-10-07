using System.Collections.Generic;
using Pubquiz.Domain.Models;

namespace Pubquiz.Logic.Tools
{
    public static class TestUsers
    {
        public static List<User> GetUsers()
        {
            var users = new List<User>
            {
                new User {UserName = "Admin", Password = "secret123", UserRole = UserRole.Admin},
                new User {UserName = "Quiz master 1", Password = "qm1", UserRole = UserRole.QuizMaster},
                new User {UserName = "Quiz master 2", Password = "qm2", UserRole = UserRole.QuizMaster}
            };


            return users;
        }
    }
}