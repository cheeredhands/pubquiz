using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Pubquiz.Persistence
{
    /// <summary>
    ///     Interface for retrieving data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICollection<T> where T : Model, new()
    {
        /// <summary>
        ///     Get all objects, linq queries can be done on the IQueryable collection.
        /// </summary>
        /// <returns></returns>
        IQueryable<T> AsQueryable();

        /// <summary>
        ///     Get document.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> GetAsync(Guid id);

        /// <summary>
        /// Gets one or more documents identified by the supplied ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAsync(params Guid[] ids);

        /// <summary>
        ///     Update a document.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(T document);

        /// <summary>
        ///     Add document.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        Task<T> AddAsync(T document);

        /// <summary>
        ///     Delete document.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        ///     Gets the number of documents in the collection
        /// </summary>
        /// <returns></returns>
        Task<long> GetCountAsync();

        /// <summary>
        ///     Gets the number of documents in the collection that satisfy the filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<long> GetCountAsync(Expression<Func<T, bool>> filter);

        /// <summary>
        ///     True if the collection contains documents.
        /// </summary>
        /// <returns></returns>
        Task<bool> AnyAsync();

        /// <summary>
        ///     True if the collection contains documents that satisfy the filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> filter);

        /// <summary>
        ///     Get the first or default in the collection.
        /// </summary>
        /// <returns></returns>
        Task<T> FirstOrDefaultAsync();

        /// <summary>
        /// Get the first or default in the collection that satisfies the filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filter);
    }
}