namespace Pubquiz.Repository.Mongo
{
    public class MongoDatabaseOptions : IMongoDatabaseOptions
    {
        public string DatabaseName { get; }
        public string ConnectionString { get; }

        public MongoDatabaseOptions(string databaseName, string connectionString)
        {
            DatabaseName = databaseName;
            ConnectionString = connectionString;
        }

        public bool FlagDelete { get; set; }
        public bool TimeLoggingEnabled { get; set; }
    }
}
