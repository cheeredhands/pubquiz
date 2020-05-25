using Pubquiz.Domain.Models;

namespace Pubquiz.Logic.Messages
{
    public class QmTeamRegistered
    {
        public Team Team { get; set; }
        public string GameId { get; set; }
    }
}