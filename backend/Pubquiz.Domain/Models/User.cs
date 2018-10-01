using System;
using Pubquiz.Persistence;

namespace Pubquiz.Domain.Models
{
    public class User : Model
    {
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }

        public string RecoveryCode { get; set; }
    }
}