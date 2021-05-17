namespace Pubquiz.WebApi.Models
{
    public class RegisterForGameResponse : ApiResponse
    {
        public string Jwt { get; set; }
        public string TeamId { get; set; }
        public string Name { get; set; }
        public string MemberNames { get; set; }
        public string GameId { get; set; }
        public string RecoveryCode { get; set; }
    }
}