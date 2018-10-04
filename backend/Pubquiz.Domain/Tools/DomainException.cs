using System;

namespace Pubquiz.Domain.Tools
{
    public class DomainException : Exception
    {
        public readonly int ErrorCode;
        public readonly bool IsBadRequest;

        public DomainException(string message, bool isBadRequest) : base(message)
        {
            IsBadRequest = isBadRequest;
        }
        
        public DomainException(int errorCode, string message, bool isBadRequest) : base(message)
        {
            ErrorCode = errorCode;
            IsBadRequest = isBadRequest;            
        }
    }
}