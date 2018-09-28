namespace Pubquiz.Repository
{
    public interface IRepositoryOptions
    {
        bool FlagDelete { get; set; }
        bool TimeLoggingEnabled { get; set; }
    }
}