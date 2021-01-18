using MediatR;
using Pubquiz.Domain.Models;

namespace Pubquiz.Logic.Requests.Notifications
{
    /// <summary>
    /// Command to change the <see cref="Team"/> name.
    /// </summary>
    public class ChangeTeamNameNotification : INotification
    {
        public string TeamId;
        public string NewName;
    }
}