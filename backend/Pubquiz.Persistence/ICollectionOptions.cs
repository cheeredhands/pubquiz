namespace Pubquiz.Persistence
{
    public interface ICollectionOptions
    {
        bool FlagDelete { get; set; }
        bool TimeLoggingEnabled { get; set; }
    }
}