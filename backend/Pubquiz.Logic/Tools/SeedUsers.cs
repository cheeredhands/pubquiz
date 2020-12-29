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
                    Id = Guid.Parse("12AC11CB-C6BB-499D-88A4-4325EF5F8A05").ToShortGuidString(),
                    UserName = "Admin",
                    Password = "secret123",
                    UserRole = UserRole.Admin
                },
                new User
                {
                    Id = Guid.Parse("637AF690-71C7-47D0-B5D9-E1C15A1931E8").ToShortGuidString(),
                    UserName = "Quiz master 1",
                    Password = "qm1",
                    UserRole = UserRole.QuizMaster
                }
            };

            return users;
        }
    }
}