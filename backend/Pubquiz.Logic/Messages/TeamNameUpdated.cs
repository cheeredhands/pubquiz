namespace Pubquiz.Logic.Messages
{
    public class TeamNameUpdated
    {
        public string TeamId { get; }
        public string GameId { get; }
        public string OldTeamName { get; }
        public string TeamName { get; }

        public TeamNameUpdated(string teamId, string gameId, string oldTeamName, string teamName)
        {
            TeamId = teamId;
            GameId = gameId;
            OldTeamName = oldTeamName;
            TeamName = teamName;
        }
    }
}