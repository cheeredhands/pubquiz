using MediatR;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.ViewModels;
using Pubquiz.Logic.Tools;

namespace Pubquiz.Logic.Requests.Queries
{
    /// <summary>
    /// Query to get the <see cref="QmLobbyViewModel"/> for a specific <see cref="User"/>.
    /// </summary>
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "UserId")]
    public class QmLobbyViewModelQuery : IRequest<QmLobbyViewModel>
    {
        public string UserId { get; set; }
    }
}