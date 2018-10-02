using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Pubquiz.Persistence.Extensions;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

//This collection must be used in combination with caching. If the cache decorator
//Never clears, this can be used as an in memory collection.
namespace Pubquiz.Persistence.NoAction
{
    /// <summary>
    ///     Base collection for no action
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NoActionCollection<T> : ICollection<T> where T : Model, new()
    {
        public IQueryable<T> AsQueryable() => new ConcurrentBag<T>().Clone().AsQueryable();
        public async Task<T> GetAsync(Guid id) => null;
        public async Task<bool> UpdateAsync(T document) => true;
        public async Task<T> AddAsync(T document) => document.Clone();
        public async Task<bool> DeleteAsync(Guid id) => true;
        public async Task<long> GetCountAsync() => 0;
        public async Task<long> GetCountAsync(Expression<Func<T, bool>> filter) => 0;
        public async Task<bool> AnyAsync() => false;
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter) => false;
        public async Task<T> FirstOrDefaultAsync() => new T();
        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filter) => new T();
    }
}