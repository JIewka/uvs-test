using UVS_Assignment.Cli.Enums;

namespace UVS_Assignment.Cli.Models
{
    public class ParsedCommand
    {
        public CommandType CommandType { get; set; }

        public GetEmployeeCommand? GetEmployeeCommand { get; set; }
        public SetEmployeeCommand? SetEmployeeCommand { get; set; }
    }
}
