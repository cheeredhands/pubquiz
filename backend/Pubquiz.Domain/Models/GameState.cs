namespace Pubquiz.Domain.Models
{
    public enum GameState
    {
        Closed,
        Open, // e.g. open for registration
        Running, // InSession? Started?
        Paused,
        Finished // Ended?
    }
}