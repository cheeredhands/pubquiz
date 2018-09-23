using System;
using Microsoft.AspNetCore.Identity;
using Pubquiz.Domain.Models;

namespace Pubquiz.WebApi.Helpers
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        /// <summary>
        /// The invite or recovery code that was used.
        /// </summary>
        public string Code { get; set; }

        public string TeamName { get; set; }

        public static ApplicationUser FromUser(User user)
        {
            return new ApplicationUser
            {
                Id = user.Id,
                UserName = user.UserName,
                NormalizedUserName = user.NormalizedUserName
            };
        }
    }
}