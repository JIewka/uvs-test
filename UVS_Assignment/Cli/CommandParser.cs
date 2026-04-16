using UVS_Assignment.Cli.Enums;
using UVS_Assignment.Cli.Models;

namespace UVS_Assignment.Cli
{
    public class CommandParser : ICommandParser
    {
        public ParsedCommand Parse(string[] args)
        {
            if (args.Length == 0)
                throw new ArgumentException("No command provided.");

            var command = args[0];
            var parsedArgs = ParseArguments(args.Skip(1).ToArray());

            return command switch
            {
                "get-employee" => new ParsedCommand
                {
                    CommandType = CommandType.GET_EMPLOYEE,
                    GetEmployeeCommand = new GetEmployeeCommand
                    {
                        EmployeeId = GetIntArgument(parsedArgs, "--employeeId")
                    }
                },

                "set-employee" => new ParsedCommand
                {
                    CommandType = CommandType.SET_EMPLOYEE,
                    SetEmployeeCommand = new SetEmployeeCommand
                    {
                        EmployeeId = GetIntArgument(parsedArgs, "--employeeId"),
                        EmployeeName = GetStringArgument(parsedArgs, "--employeeName"),
                        EmployeeSalary = GetIntArgument(parsedArgs, "--employeeSalary")
                    }
                },

                _ => throw new ArgumentException($"Unknown command: {command}")
            };
        }

        private static Dictionary<string, string> ParseArguments(string[] args)
        {
            var parsedArgs = new Dictionary<string, string>(StringComparer.Ordinal);

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (!arg.StartsWith("--", StringComparison.Ordinal))
                    throw new ArgumentException($"Unknown argument: {arg}");

                if (parsedArgs.ContainsKey(arg))
                    throw new ArgumentException($"Duplicate argument: {arg}");

                if (i + 1 >= args.Length || args[i + 1].StartsWith("--", StringComparison.Ordinal))
                    throw new ArgumentException($"Missing argument: {arg}");

                parsedArgs[arg] = args[i + 1];
                i++;
            }

            return parsedArgs;
        }

        private static string GetStringArgument(Dictionary<string, string> options, string argumentName)
        {
            if (!options.TryGetValue(argumentName, out var value))
                throw new ArgumentException($"Missing argument: {argumentName}");

            return value;
        }

        private static int GetIntArgument(Dictionary<string, string> options, string argumentName)
        {
            var value = GetStringArgument(options, argumentName);

            if (!int.TryParse(value, out var result))
                throw new ArgumentException($"Argument {argumentName} must be an integer.");

            return result;
        }
    }
}
