using System.Collections.Generic;
using System.Linq;

namespace Pubquiz.Domain.ViewModels
{
    public class QmTeamFeedViewModel
    {
        public List<TeamViewModel> Teams { get; set; }

        public QmTeamFeedViewModel(IEnumerable<TeamViewModel> teams)
        {
            Teams = teams.ToList();
        }
    }
}