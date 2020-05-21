using System.Collections.Generic;
using System.Linq;
using Pubquiz.Domain.Models;

namespace Pubquiz.Domain.ViewModels
{
    public class QmTeamRankingViewModel
    {
        public List<TeamViewModel> Teams { get; set; }

        public QmTeamRankingViewModel(IEnumerable<TeamViewModel> teams)
        {
            Teams = teams.ToList();
        }
    }
}