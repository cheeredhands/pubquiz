using System.Threading.Tasks;

namespace Pubquiz.Domain
{
    /// <summary>
    /// A command request that doesn't have a return value e.g. fire-and-forget.
    /// </summary>
    public abstract class Notification : Request
    {
        public Task Execute()
        {
            return DoExecute();
        }

        protected abstract Task DoExecute();
    }
}