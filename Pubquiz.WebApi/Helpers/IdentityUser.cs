using System;
using Microsoft.AspNetCore.Identity;
using Pubquiz.Domain.Models;

namespace Pubquiz.WebApi.Helpers
{
    public class IdentityUser : IdentityUser<Guid>
    {
        /// <summary>
        /// The invite or recovery code that was used.
        /// </summary>
        public string Code { get; set; }

        public static IdentityUser FromUser(User user)
        {
            return new IdentityUser
            {
                Id = user.Id,
                UserName = user.UserName,
                NormalizedUserName = user.NormalizedUserName
            };
        }
    }
}