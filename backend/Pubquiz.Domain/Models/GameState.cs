using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Pubquiz.Domain.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GameState
    {
        Closed,
        Open, 
        Running, 
        Reviewing,
        Paused,
        Finished,
        Deleted
    }
}