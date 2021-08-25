namespace Pubquiz.Persistence.InMemory
{
    public class InMemoryDatabaseOptions : IInMemoryDatabaseOptions
    {
        public bool FlagDelete { get; set; } = true;
        public bool TimeLoggingEnabled { get; set; }
    }
}