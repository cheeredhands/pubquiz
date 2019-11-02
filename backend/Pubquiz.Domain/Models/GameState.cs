using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Pubquiz.Domain.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GameState
    {
        Closed,
        Open, // e.g. open for registration
        Running, // InSession? Started?
        Paused,
        Finished // Ended?
    }
}