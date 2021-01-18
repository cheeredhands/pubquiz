using MediatR;
using Pubquiz.Domain.Models;

namespace Pubquiz.Logic.Messages
{
    public class QmTeamRegistered: INotification

    {
    public Team Team { get; set; }
    public string GameId { get; set; }
    }
}