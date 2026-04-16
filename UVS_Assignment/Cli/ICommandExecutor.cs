namespace UVS_Assignment.Cli
{
    public interface ICommandExecutor
    {
        Task ExecuteAsync(string[] args);
    }
}
