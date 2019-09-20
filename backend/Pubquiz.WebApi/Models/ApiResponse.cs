using System;
using System.Collections.Generic;
using Pubquiz.Domain.Models;

namespace Pubquiz.WebApi.Models
{
    public class ApiResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }

    public class WhoAmiResponse : ApiResponse
    {
        public string UserName { get; set; }
        public Guid UserId { get; set; }
        public Guid CurrentGameId { get; set; }
        public UserRole UserRole { get; set; }
    }

    public class RegisterForGameResponse : ApiResponse
    {
        public Guid TeamId { get; set; }
        public string TeamName { get; set; }
        public string MemberNames { get; set; }
    }

    public class LoginResponse : ApiResponse
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public List<Guid> GameIds { get; set; }
    }

    public class SelectGameResponse : ApiResponse
    {
        public Guid GameId { get; set; }
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
}