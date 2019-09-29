using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Persistence.Extensions;

namespace Pubquiz.Logic.Tools
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Get the <see cref="UserRole"/> from the <see cref="ClaimTypes.Role"/> <see cref="Claim"/>.
        /// </summary>
        /// <param name="claimsPrincipal"></param>
        /// <returns></returns>
        public static UserRole GetUserRole(this ClaimsPrincipal claimsPrincipal)
        {
            var claim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            if (claim == null)
            {
                throw new DomainException(ErrorCodes.NoRoleClaimForUser, "The user doesn't have a Role claim.", true);
            }

            if (!Enum.TryParse(claim.Value, out UserRole result))
            {
                throw new DomainException(ErrorCodes.NoRoleClaimForUser, "The user doesn't have a valid Role claim.",
                    true);
            }

            return result;
        }

        public static string GetCurrentGameId(this ClaimsPrincipal claimsPrincipal)
        {
            var claim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "CurrentGame");
            if (claim == null)
            {
                throw new DomainException(ErrorCodes.NoCurrentGameIdClaimForUser,
                    "The user doesn't have a Current Game claim.", true);
            }

            if (!claim.Value.TryDecodeToGuid(out _))//  !Guid.TryParse(claim.Value, out var result))
            {
                throw new DomainException(ErrorCodes.NoCurrentGameIdClaimForUser,
                    "The user doesn't have a valid Current Game claim.",
                    true);
            }

            return claim.Value;
        }

        /// <summary>
        /// Get the <see cref="Guid"/> of the logged in <see cref="User"/> from the <see cref="ClaimTypes.NameIdentifier"/> <see cref="Claim"/>.
        /// </summary>
        /// <param name="claimsPrincipal">The logged in user.</param>
        /// <returns>The guid of the user if the claim exists, otherwise <see cref="Guid.Empty"/></returns>
        public static string GetId(this ClaimsPrincipal claimsPrincipal)
        {
            var idClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (idClaim != null)
            {
                var idString = idClaim.Value;
                if (idString.TryDecodeToGuid(out _))
                {
                    return idString;
                }
            }

            return string.Empty;
        }
    }
}