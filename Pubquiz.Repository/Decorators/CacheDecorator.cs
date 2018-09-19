using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Pubquiz.Repository.Extensions;
using Pubquiz.Repository.Helpers;

namespace Pubquiz.Repository.Decorators
{
    /// <summary>
    ///     Decorator to use memory cache
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CacheDecorator<T> : RepositoryDecoratorBase<T> where T : Model, new()
    {
        private readonly string _collectionKey = "CachedCollection";
        private readonly object _lockCollection = new object();
        private readonly MemoryCacheEntryOptions _memoryCacheOptions;
        private readonly bool _neverRemove;

        /// <inheritdoc />
        public CacheDecorator(IMemoryCache memoryCache, bool neverRemove, IRepository<T> decoree) : base(memoryCache, decoree)
        {
            _neverRemove = neverRemove;
            if (neverRemove)
            {
                _collectionKey = string.Concat(_collectionKey, typeof(T));
            }

            var cacheAttribute = typeof(T).GetTypeInfo().GetCustomAttribute<CacheAttribute>();
            var cacheTimeInSeconds = cacheAttribute?.SecondsToCache ?? 300;
            _memoryCacheOptions = neverRemove
                ? new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove)
                : new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(cacheTimeInSeconds));
        }

        private ConcurrentDictionary<string, Model> Collection
        {
            get
            {
                if (MemoryCache == null) return new ConcurrentDictionary<string, Model>();
                return MemoryCache.TryGetValue(_collectionKey, out ConcurrentDictionary<string, Model> coll)
                    ? coll
                    : new ConcurrentDictionary<string, Model>();
            }
        }

        /// <summary>
        ///     Override AsQueryable, NODE this could fail on a loadbalanced server because the collection wouldn't be filled
        ///     properly
        ///     The never remove option should only be used for development purposes.
        /// </summary>
        /// <returns></returns>
        public override IQueryable<T> AsQueryable()
        {
            return _neverRemove ? Collection.Values.OfType<T>().Clone().AsQueryable() : base.AsQueryable();
        }

        /// <inheritdoc />
        public override async Task<T> GetAsync(Guid id)
        {
            if (MemoryCache == null) return await base.GetAsync(id);
            var ret = MemoryCache.TryGetValue($"{typeof(T)}-{id}", out T returnValue)
                ? returnValue
                : await base.GetAsync(id);
            return ret.Clone();
        }

        /// <summary>
        ///     Add objects to cache before adding to database
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public override async Task<T> AddAsync(T document)
        {
            if (MemoryCache == null) return await base.AddAsync(document);
            var key = $"{typeof(T)}-{document.Id}";
            var doc = document.Clone();
            if (_neverRemove)
            {
                lock (_lockCollection)
                {
                    var coll = Collection;
                    if (!coll.ContainsKey(key)) coll.TryAdd(key, doc);
                    MemoryCache.Set(_collectionKey, coll, _memoryCacheOptions);
                }
            }
            MemoryCache.Set(key, doc, _memoryCacheOptions);
            var ret = await base.AddAsync(doc);
            return ret.Clone();
        }

        /// <summary>
        ///     Update and edit cache before update in the database
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(T document)
        {
            if (MemoryCache == null) return await base.UpdateAsync(document);
            var key = $"{typeof(T)}-{document.Id}";
            var doc = document.Clone();
            if (_neverRemove)
            {
                lock (_lockCollection)
                {
                    if (Collection.ContainsKey(key))
                    {
                        Collection[key] = doc;
                        MemoryCache.Set(_collectionKey, Collection, _memoryCacheOptions);
                    }
                }
            }
            MemoryCache.Set(key, doc, _memoryCacheOptions);
            return await base.UpdateAsync(document);
        }

       public override async Task<bool> DeleteAsync(Guid id)
        {
            if (MemoryCache == null) return await base.DeleteAsync(id);
            var key = $"{typeof(T)}-{id}";
            if (_neverRemove)
            {
                if (Collection.ContainsKey(key))
                {
                    lock (_lockCollection)
                    {
                        Collection.TryRemove(key, out _);
                        MemoryCache.Set(_collectionKey, Collection, _memoryCacheOptions);
                    }
                }
            }
            MemoryCache.Remove(key);
            return await base.DeleteAsync(id);
        }

        public override async Task<long> GetCountAsync()
        {
            if (MemoryCache == null) return await base.GetCountAsync();
            return _neverRemove ? Collection.Count : await base.GetCountAsync();
        }

        public override async Task<T> FirstOrDefaultAsync()
        {
            if (MemoryCache == null) return await base.FirstOrDefaultAsync();
            return _neverRemove
                ? Collection.Values.OfType<T>().AsQueryable().FirstOrDefault().Clone()
                : await base.FirstOrDefaultAsync();
        }

        public override async Task<bool> AnyAsync()
        {
            if (MemoryCache == null) return await base.AnyAsync();
            return _neverRemove ? Collection.Any() : await base.AnyAsync();
        }

        public override async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            if (MemoryCache == null) return await base.AnyAsync();
            return _neverRemove ? Collection.Values.OfType<T>().AsQueryable().Any(filter) : await base.AnyAsync();
        }

        public override async Task<long> GetCountAsync(Expression<Func<T, bool>> filter)
        {
            if (MemoryCache == null) return await base.GetCountAsync();
            return _neverRemove
                ? Collection.Values.OfType<T>().AsQueryable().Count(filter)
                : await base.GetCountAsync();
        }
    }
}