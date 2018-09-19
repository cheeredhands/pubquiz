using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Pubquiz.Repository
{
    /// <summary>
    ///     Interface for retrieving data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : Model, new()
    {
        /// <summary>
        ///     Get all objects, linq queries can be done on the IQueryable collecion.
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
        ///     Gets the number of documents in the repository
        /// </summary>
        /// <returns></returns>
        Task<long> GetCountAsync();

        /// <summary>
        ///     Gets the number of documents in the repository that satisfy the filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<long> GetCountAsync(Expression<Func<T, bool>> filter);

        /// <summary>
        ///     True if the repository contains documents.
        /// </summary>
        /// <returns></returns>
        Task<bool> AnyAsync();

        /// <summary>
        ///     True if the repository contains documents that satisfy the filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> filter);

        /// <summary>
        ///     Get the first or default in the collection.
        /// </summary>
        /// <returns></returns>
        Task<T> FirstOrDefaultAsync();
    }
}