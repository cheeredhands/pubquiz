using System;

namespace Pubquiz.Domain
{
    public class DomainException : Exception
    {
        public readonly ResultCode ResultCode;
        public readonly bool IsBadRequest;

        public DomainException(string message, bool isBadRequest) : base(message)
        {
            IsBadRequest = isBadRequest;
        }
        
        public DomainException(ResultCode resultCode, string message, bool isBadRequest) : base(message)
        {
            ResultCode = resultCode;
            IsBadRequest = isBadRequest;            
        }
    }
}