namespace Pubquiz.Repository.Mongo
{
    public interface IMongoDatabaseOptions: IRepositoryOptions
    {
        string ConnectionString { get; }
        string DatabaseName { get; }
    }
}