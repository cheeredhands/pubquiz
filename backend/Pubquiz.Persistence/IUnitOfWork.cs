namespace Pubquiz.Persistence
{
    /// <summary>
    ///     Interface for Unit of Work
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// ActorId 
        /// </summary>
        string ActorId { get; set; }

        /// <summary>
        ///     Get the collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ICollection<T> GetCollection<T>() where T : Model, new();
    }
}