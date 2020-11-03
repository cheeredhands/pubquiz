namespace Pubquiz.Logic.Messages
{
    public class TeamDisconnected
    {
        public string TeamId { get; }
        public string GameId { get; }
        public string Name { get; }
        public int ConnectionCount { get; }
        
        public TeamDisconnected(string teamId, string name, string gameId, int connectionCount)
        {
            TeamId = teamId;
            Name = name;
            GameId = gameId;
            ConnectionCount = connectionCount;
        }
    }
}