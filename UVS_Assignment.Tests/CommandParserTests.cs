using UVS_Assignment.Cli;
using UVS_Assignment.Cli.Enums;

namespace UVS_Assignment.Tests
{
    public class CommandParserTests
    {
        private readonly CommandParser _parser;

        public CommandParserTests()
        {
            _parser = new CommandParser();
        }

        [Fact]
        public void Parse_NoArgumentsProvided_ThrowArgumentException()
        {
            // Arrange
            var args = Array.Empty<string>();

            // Act
            var action = () => _parser.Parse(args);

            // Assert
            var exception = Assert.Throws<ArgumentException>(action);
            Assert.Equal("No command provided.", exception.Message);
        }

        [Fact]
        public void Parse_GetEmployeeCommand_ParseGetEmployeeCommand()
        {
            // Arrange
            var args = new[] { "get-employee", "--employeeId", "1" };

            // Act
            var result = _parser.Parse(args);

            // Assert
            Assert.Equal(CommandType.GET_EMPLOYEE, result.CommandType);
            Assert.NotNull(result.GetEmployeeCommand);
            Assert.Equal(1, result.GetEmployeeCommand!.EmployeeId);
        }

        [Fact]
        public void Parse_SetEmployeeCommand_ParseSetEmployeeCommand()
        {
            // Arrange
            var args = new[] 
            { 
                "set-employee", 
                "--employeeId", "1", 
                "--employeeName", "John", 
                "--employeeSalary", "123" 
            };

            // Act
            var result = _parser.Parse(args);

            // Assert
            Assert.Equal(CommandType.SET_EMPLOYEE, result.CommandType);
            Assert.NotNull(result.SetEmployeeCommand);
            Assert.Equal(1, result.SetEmployeeCommand!.EmployeeId);
            Assert.Equal("John", result.SetEmployeeCommand.EmployeeName);
            Assert.Equal(123, result.SetEmployeeCommand.EmployeeSalary);
        }

        [Fact]
        public void Parse_UnknownCommand_ThrowArgumentException()
        {
            // Arrange
            var args = new[] { "delete-employee" };

            // Act
            var action = () => _parser.Parse(args);

            // Assert
            var exception = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Unknown command: delete-employee", exception.Message);
        }

        [Fact]
        public void Parse_GetEmployeeCommandWhenMissingEmployeeId_ThrowArgumentException()
        {
            // Arrange
            var args = new[] { "get-employee" };

            // Act
            var action = () => _parser.Parse(args);

            // Assert
            var exception = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Missing argument: --employeeId", exception.Message);
        }

        [Fact]
        public void Parse_SetEmployeeCommandWhenDuplicateArgument_ThrowArgumentException()
        {
            // Arrange
            var args = new[]
            {
                "set-employee",
                "--employeeId", "1",
                "--employeeId", "2",
                "--employeeName", "John",
                "--employeeSalary", "123"
            };

            // Act
            var action = () => _parser.Parse(args);

            // Assert
            var exception = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Duplicate argument: --employeeId", exception.Message);
        }

        [Fact]
        public void Parse_SetEmployeeCommandWhenMissingArgumentValue_ThrowArgumentException()
        {
            // Arrange
            var args = new[]
            {
                "set-employee",
                "--employeeId", "1",
                "--employeeName",
                "--employeeSalary", "123"
            };

            // Act
            var action = () => _parser.Parse(args);

            // Assert
            var exception = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Missing argument: --employeeName", exception.Message);
        }

        [Fact]
        public void Parse_SetEmployeeCommandWhenUnknownArgument_ThrowArgumentException()
        {
            // Arrange
            var args = new[]
            {
                "set-employee",
                "--employeeId", "1",
                "unknown argument",
                "--employeeName", "John",
                "--employeeSalary", "123"
            };

            // Act
            var action = () => _parser.Parse(args);

            // Assert
            var exception = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Unknown argument: unknown argument", exception.Message);
        }

        [Fact]
        public void Parse_SetEmployeeCommandWhenEmployeeSalaryIsNotInteger_ThrowArgumentException()
        {
            // Arrange
            var args = new[]
            {
                "set-employee",
                "--employeeId", "1",
                "--employeeName", "John",
                "--employeeSalary", "abc"
            };

            // Act
            var action = () => _parser.Parse(args);

            // Assert
            var exception = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Argument --employeeSalary must be an integer.", exception.Message);
        }
    }
}
