using Pubquiz.Domain;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Pubquiz.WebApi.Models
{
    public class ApiResponse
    {
        public ResultCode Code { get; set; }
        public string Message { get; set; }
    }
}