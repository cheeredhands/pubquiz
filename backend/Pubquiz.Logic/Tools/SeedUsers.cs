using System;
using System.Collections.Generic;
using Pubquiz.Domain.Models;
using Pubquiz.Persistence.Extensions;

namespace Pubquiz.Logic.Tools
{
    public static class SeedUsers
    {
        public static List<User> GetUsers()
        {
            var users = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid().ToShortGuidString(),
                    UserName = "Admin",
                    Password = "secret123",
                    UserRole = UserRole.Admin
                },
                new User
                {
                    Id = Guid.NewGuid().ToShortGuidString(),
                    UserName = "Quiz master 1",
                    Password = "qm1",
                    UserRole = UserRole.QuizMaster
                }
            };

            return users;
        }
    }
}