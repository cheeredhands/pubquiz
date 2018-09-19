using System;
using Pubquiz.Domain.Models;
using Pubquiz.Repository;

namespace Pubquiz.Domain.Tools
{
    public static class Helpers
    {
        public static string GenerateSessionRecoveryCode(IRepository<Team> teamRepository, Guid gameId)
        {
            return "flatulent extra dof";
        }
    }
}