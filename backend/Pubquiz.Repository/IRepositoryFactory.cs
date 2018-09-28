using System;

namespace Pubquiz.Repository
{
    /// <summary>
    ///     Interface for repository factory
    /// </summary>
    public interface IRepositoryFactory
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
        IRepository<T> GetRepository<T>() where T : Model, new();
    }
}