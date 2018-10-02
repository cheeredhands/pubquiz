using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Pubquiz.Persistence.Decorators
{
    /// <summary>
    ///     Base class for collection decorators.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CollectionDecoratorBase<T> : ICollection<T> where T : Model, new()
    {
        private readonly ICollection<T> _decoree;
        protected readonly IMemoryCache MemoryCache;
        protected ILoggerFactory LoggerFactory;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="memoryCache"></param>
        /// <param name="decoree"></param>
        protected CollectionDecoratorBase(IMemoryCache memoryCache, ICollection<T> decoree)
        {
            _decoree = decoree;
            MemoryCache = memoryCache;
        }

        public virtual async Task<T> AddAsync(T document) =>
            await _decoree.AddAsync(document);

        public virtual async Task<bool> DeleteAsync(Guid id) =>
            await _decoree.DeleteAsync(id);

        public virtual IQueryable<T> AsQueryable() =>
            _decoree.AsQueryable();

        public virtual async Task<bool> UpdateAsync(T document) =>
            await _decoree.UpdateAsync(document);

        public virtual async Task<T> GetAsync(Guid id) =>
            await _decoree.GetAsync(id);

        public virtual async Task<long> GetCountAsync() =>
            await _decoree.GetCountAsync();

        public virtual async Task<long> GetCountAsync(Expression<Func<T, bool>> filter) =>
            await _decoree.GetCountAsync(filter);

        public virtual async Task<bool> AnyAsync() =>
            await _decoree.AnyAsync();

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> filter) =>
            await _decoree.AnyAsync(filter);

        public virtual async Task<T> FirstOrDefaultAsync() =>
            await _decoree.FirstOrDefaultAsync();

        public virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filter) =>
            await _decoree.FirstOrDefaultAsync(filter);
    }
}