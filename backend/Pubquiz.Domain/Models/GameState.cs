using System.Text.Json.Serialization;

namespace Pubquiz.Domain.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GameState
    {
        Closed,
        Open, 
        Running, 
        Reviewing,
        Paused,
        Finished
    }
}