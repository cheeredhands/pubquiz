namespace Pubquiz.Logic.Messages
{
    public class TeamNameUpdated
    {
        public string TeamId { get; }
        public string GameId { get; }
        public string OldName { get; }
        public string Name { get; }

        public TeamNameUpdated(string teamId, string gameId, string oldName, string name)
        {
            TeamId = teamId;
            GameId = gameId;
            OldName = oldName;
            Name = name;
        }
    }
}