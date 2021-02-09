using System.Collections.Generic;
using MediatR;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;

namespace Pubquiz.Logic.Requests.Queries
{
    /// <summary>
    /// Query to get the available Games for the <see cref="User"/>.
    /// </summary>
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "UserId")]
    public class GetGamesQuery : IRequest<List<Game>>
    {
        public string UserId { get; set; }
    }
}