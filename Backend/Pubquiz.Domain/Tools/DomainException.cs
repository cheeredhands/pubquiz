using System;

namespace Pubquiz.Domain.Tools
{
    public class DomainException : Exception
    {
        public readonly bool IsBadRequest;

        public DomainException(string message, bool isBadRequest) : base(message)
        {
            IsBadRequest = isBadRequest;
        }
    }
}