namespace Pubquiz.Persistence.MongoDb
{
    public interface IMongoDbDatabaseOptions: ICollectionOptions
    {
        string ConnectionString { get; }
        string DatabaseName { get; }
    }
}