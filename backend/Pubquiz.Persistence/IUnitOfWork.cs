using System;

namespace Pubquiz.Persistence
{
    /// <summary>
    ///     Interface for repository factory
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// ActorId 
        /// </summary>
        Guid? ActorId { get; set; }

        /// <summary>
        ///     Get the repository
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ICollection<T> GetCollection<T>() where T : Model, new();

        /// <summary>
        /// Commit the unit of work (transaction).
        /// </summary>
        void Commit();

        /// <summary>
        /// Cancel the unit of work (abort the transaction).
        /// </summary>
        void Abort();
    }
}