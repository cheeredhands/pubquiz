namespace Pubquiz.Logic.Messages
{
    public class TeamRegistered
    {
        public string TeamId { get; }
        public string GameId { get; }
        public string TeamName { get; }
        public string MemberNames { get; }

        public TeamRegistered(string teamId, string teamName, string gameId, string memberNames)
        {
            TeamId = teamId;
            TeamName = teamName;
            GameId = gameId;
            MemberNames = memberNames;
        }
    }
}