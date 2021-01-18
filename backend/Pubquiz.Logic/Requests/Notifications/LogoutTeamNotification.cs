using MediatR;

namespace Pubquiz.Logic.Requests.Notifications
{
    public class LogoutTeamNotification : INotification
    {
        public string TeamId { get; set; }
    }
}