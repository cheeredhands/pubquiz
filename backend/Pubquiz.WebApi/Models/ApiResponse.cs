using System.Collections.Generic;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Pubquiz.WebApi.Models
{
    public class ApiResponse
    {
        public ResultCode Code { get; set; }
        public string Message { get; set; }
    }

    public class WhoAmiResponse : ApiResponse
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string MemberNames { get; set; }
        public string CurrentGameId { get; set; }
        public List<GameRef> GameRefs { get; set; }
        public GameState GameState { get; set; }
        public UserRole UserRole { get; set; }
    }

    public class RegisterForGameResponse : ApiResponse
    {
        public string Jwt { get; set; }
        public string TeamId { get; set; }
        public string Name { get; set; }
        public string MemberNames { get; set; }
        public string GameId { get; set; }
        public string RecoveryCode { get; set; }
    }

    public class LoginResponse : ApiResponse
    {
        public string Jwt { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string CurrentGameId { get; set; }
        public List<GameRef> GameRefs { get; set; }
    }

    public class SelectGameResponse : ApiResponse
    {
        public string GameId { get; set; }
    }

    public class TestAuthResponse : ApiResponse
    {
        public List<Team> Teams { get; set; }
    }

    public class ChangeTeamNameResponse : ApiResponse
    {
        public string TeamName { get; set; }
    }

    public class ChangeTeamMembersResponse : ApiResponse
    {
        public string TeamMembers { get; set; }
    }

    public class NavigateItemResponse : ApiResponse
    {
        public string QuizItemId { get; set; }
    }

    public class ImportZippedExcelQuizResponse : ApiResponse
    {
        public string QuizId { get; set; }
    }
}