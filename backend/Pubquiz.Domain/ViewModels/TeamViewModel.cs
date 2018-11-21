using System;

namespace Pubquiz.Domain.ViewModels
{
    public class TeamViewModel
    {
        public Guid TeamId { get; set; }
        public string TeamName { get; set; }
        public string MemberNames { get; set; }
    }
}
