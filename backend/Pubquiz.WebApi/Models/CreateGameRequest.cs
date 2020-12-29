namespace Pubquiz.WebApi.Models
{
    public class CreateGameRequest
    {
        public string QuizId { get; set; }
        public string GameTitle { get; set; }
        public string InviteCode { get; set; }
    }
}