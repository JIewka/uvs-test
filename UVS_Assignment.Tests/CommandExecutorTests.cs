using Moq;
using UVS_Assignment.Cli;
using UVS_Assignment.Cli.Enums;
using UVS_Assignment.Cli.Models;
using UVS_Assignment.Dtos;
using UVS_Assignment.Services;

namespace UVS_Assignment.Tests
{
    public class CommandExecutorTests
    {
        private readonly Mock<ICommandParser> _commandParserMock;
        private readonly Mock<IEmployeeService> _employeeServiceMock;
        private readonly CommandExecutor _commandExecutor;

        public CommandExecutorTests()
        {
            _commandParserMock = new Mock<ICommandParser>();
            _employeeServiceMock = new Mock<IEmployeeService>();
            _commandExecutor = new CommandExecutor(_commandParserMock.Object, _employeeServiceMock.Object);
        }

        private void SetupGetEmployeeCommand(int employeeId = 1)
        {
            _commandParserMock
                .Setup(parser => parser.Parse(It.IsAny<string[]>()))
                .Returns(new ParsedCommand
                {
                    CommandType = CommandType.GET_EMPLOYEE,
                    GetEmployeeCommand = new GetEmployeeCommand
                    {
                        EmployeeId = employeeId
                    }
                });
        }

        private void SetupSetEmployeeCommand(
            int employeeId = 1,
            string employeeName = "John",
            int employeeSalary = 123)
        {
            _commandParserMock
                .Setup(parser => parser.Parse(It.IsAny<string[]>()))
                .Returns(new ParsedCommand
                {
                    CommandType = CommandType.SET_EMPLOYEE,
                    SetEmployeeCommand = new SetEmployeeCommand
                    {
                        EmployeeId = employeeId,
                        EmployeeName = employeeName,
                        EmployeeSalary = employeeSalary
                    }
                });
        }

        [Fact]
        public async Task ExecuteAsync_NoArgumentsProvided_PrintPossibleCommands()
        {
            // Arrange
            using var writer = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(writer);

            // Act
            try
            {   
                await _commandExecutor.ExecuteAsync(Array.Empty<string>());
            }
            finally
            {
                Console.SetOut(originalOut);
            }

            // Assert
            var output = writer.ToString();
            Assert.Contains("Possible commands:", output);
            Assert.Contains("set-employee --employeeId <id> --employeeName <name> --employeeSalary <salary>", output);
            Assert.Contains("get-employee --employeeId <id>", output);
        }

        [Fact]
        public async Task ExecuteAsync_GetEmployeeCommandWhenEmployeeExists_PrintEmployee()
        {
            // Arrange
            SetupGetEmployeeCommand();

            _employeeServiceMock
                .Setup(service => service.GetEmployeeAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EmployeeDto
                {
                    Id = 1,
                    Name = "John",
                    Salary = 123
                });

            using var writer = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(writer);

            // Act
            try
            {     
                await _commandExecutor.ExecuteAsync(new[] { "get-employee", "--employeeId", "1" });
            }
            finally
            {
                Console.SetOut(originalOut);
            }

            // Assert
            var output = writer.ToString();
            Assert.Contains("EmployeeId: 1", output);
            Assert.Contains("EmployeeName: John", output);
            Assert.Contains("EmployeeSalary: 123", output);
        }

        [Fact]
        public async Task ExecuteAsync_GetEmployeeCommandWhenEmployeeDoesNotExist_PrintNotFoundMessage()
        {
            // Arrange
            SetupGetEmployeeCommand();

            _employeeServiceMock
                .Setup(service => service.GetEmployeeAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((EmployeeDto?)null);

            using var writer = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(writer);

            // Act
            try
            {
                await _commandExecutor.ExecuteAsync(new[] { "get-employee", "--employeeId", "1" });
            }
            finally
            {
                Console.SetOut(originalOut);
            }

            // Assert
            Assert.Contains("Employee with ID 1 was not found.", writer.ToString());
        }

        [Fact]
        public async Task ExecuteAsync_SetEmployeeCommand_SaveEmployeeAndPrintSuccessMessage()
        {
            // Arrange
            SetupSetEmployeeCommand();

            using var writer = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(writer);

            // Act
            try
            {
                await _commandExecutor.ExecuteAsync(new[] { "set-employee", "--employeeId", "1", "--employeeName", "John", "--employeeSalary", "123" });
            }
            finally
            {
                Console.SetOut(originalOut);
            }

            // Assert
            _employeeServiceMock.Verify(service => service.SetEmployeeAsync(1, "John", 123, It.IsAny<CancellationToken>()), Times.Once);
            Assert.Contains("Employee saved successfully.", writer.ToString());
        }

        [Fact]
        public async Task ExecuteAsync_GetEmployeeCommandWhenCommandNotParsed_ThrowInvalidOperationException()
        {
            // Arrange
            _commandParserMock
                .Setup(parser => parser.Parse(It.IsAny<string[]>()))
                .Returns(new ParsedCommand
                {
                    CommandType = CommandType.GET_EMPLOYEE
                });

            // Act
            var action = async () => await _commandExecutor.ExecuteAsync(new[] { "get-employee" });

            // Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(action);
            Assert.Equal("Get employee command was not parsed.", exception.Message);
        }

        [Fact]
        public async Task ExecuteAsync_SetEmployeeCommandWhenCommandNotParsed_ThrowInvalidOperationException()
        {
            // Arrange
            _commandParserMock
                .Setup(parser => parser.Parse(It.IsAny<string[]>()))
                .Returns(new ParsedCommand
                {
                    CommandType = CommandType.SET_EMPLOYEE
                });

            // Act
            var action = async () => await _commandExecutor.ExecuteAsync(new[] { "set-employee" });

            // Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(action);
            Assert.Equal("Set employee command was not parsed.", exception.Message);
        }

        [Fact]
        public async Task ExecuteAsync_CommandIsUnknown_PrintUnknownCommandAndPossibleCommands()
        {
            // Arrange
            _commandParserMock
                .Setup(parser => parser.Parse(It.IsAny<string[]>()))
                .Returns(new ParsedCommand
                {
                    CommandType = CommandType.UNKNOWN
                });

            using var writer = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(writer);


            // Act
            try
            {
                await _commandExecutor.ExecuteAsync(new[] { "unknown-command" });
            }
            finally
            {
                Console.SetOut(originalOut);
            }

            // Assert
            var output = writer.ToString();
            Assert.Contains("Command is unknown.", output);
            Assert.Contains("Possible commands:", output);
        }
    }
}
