using System.Threading.Tasks;

namespace Pubquiz.Domain
{
    /// <summary>
    /// A command request.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public abstract class Command<TResponse> : Request
    {
        public Task<TResponse> Execute()
        {
            return DoExecute();
        }
        protected abstract Task<TResponse> DoExecute();
    }
}