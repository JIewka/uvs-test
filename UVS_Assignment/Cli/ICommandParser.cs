using UVS_Assignment.Cli.Models;

namespace UVS_Assignment.Cli
{
    public interface ICommandParser
    {
        ParsedCommand Parse(string[] args);
    }
}
