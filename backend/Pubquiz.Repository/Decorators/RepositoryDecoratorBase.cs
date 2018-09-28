using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Pubquiz.Repository.Decorators
{
    /// <summary>
    ///     Base class for Repository decorators.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RepositoryDecoratorBase<T> : IRepository<T> where T : Model, new()
    {
        private readonly IRepository<T> _decoree;
        protected readonly IMemoryCache MemoryCache;
        protected ILoggerFactory LoggerFactory;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="memoryCache"></param>
        /// <param name="decoree"></param>
        protected RepositoryDecoratorBase(IMemoryCache memoryCache, IRepository<T> decoree)
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

        public virtual async Task<T> GetAsync(Guid id)=>
            await _decoree.GetAsync(id);
        
        public virtual async Task<long> GetCountAsync() =>
            await _decoree.GetCountAsync();

        public virtual async Task<long> GetCountAsync(Expression<Func<T, bool>> filter) =>
            await _decoree.GetCountAsync(filter);

        public virtual async Task<bool> AnyAsync()=>
            await _decoree.AnyAsync();
        
        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> filter) =>
            await _decoree.AnyAsync(filter);
        
        public virtual async Task<T> FirstOrDefaultAsync() =>
            await _decoree.FirstOrDefaultAsync();        
    }
}