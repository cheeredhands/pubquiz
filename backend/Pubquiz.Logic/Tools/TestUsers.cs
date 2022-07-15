using System;
using System.Collections.Generic;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Persistence.Helpers;

namespace Pubquiz.Logic.Tools
{
    public static class TestUsers
    {
        public static List<User> GetUsers()
        {
            var users = new List<User>
            {
                new User
                {
                    Id = Guid.Parse("0782E94F-77FD-429E-A735-39F43EDE7234").ToShortGuidString(),
                    UserName = "Admin",
                    Password = "secret123",
                    UserRole = UserRole.Admin
                },
                new User
                {
                    Id = Guid.Parse("67C9C1EC-A9D9-4883-BFA2-6E79D72A523D").ToShortGuidString(),
                    UserName = "Quiz master 1",
                    Password = "qm1",
                    UserRole = UserRole.QuizMaster
                },
                new User
                {
                    Id = Guid.Parse("6DE1C45F-783A-418F-8673-9D3CEE6743BA").ToShortGuidString(),
                    UserName = "Quiz master 2",
                    Password = "qm2",
                    UserRole = UserRole.QuizMaster
                }
            };

            return users;
        }
    }
}