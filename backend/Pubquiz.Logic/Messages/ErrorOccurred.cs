using MediatR;
using Pubquiz.Domain;

namespace Pubquiz.Logic.Messages
{
    public class ErrorOccurred : INotification
    {
        public DomainException DomainException { get; }

        public ErrorOccurred(DomainException domainException)
        {
            DomainException = domainException;
        }
    }
}