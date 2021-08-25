using Pubquiz.Persistence.MongoDb;

namespace Pubquiz.Persistence.MongoDb
{
    public class MongoDbDatabaseOptions : IMongoDbDatabaseOptions
    {
        public string DatabaseName { get; }
        public string ConnectionString { get; }

        public MongoDbDatabaseOptions(string databaseName, string connectionString)
        {
            DatabaseName = databaseName;
            ConnectionString = connectionString;
        }

        public bool FlagDelete { get; set; }
        public bool TimeLoggingEnabled { get; set; }
    }
}
