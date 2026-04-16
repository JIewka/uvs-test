using Moq;
using UVS_Assignment.Entities;
using UVS_Assignment.Services;
using UVS_Assignment.Repositories;

namespace UVS_Assignment.Tests
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
        private readonly EmployeeService _employeeService;

        public EmployeeServiceTests()
        {
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _employeeService = new EmployeeService(_employeeRepositoryMock.Object);
        }

        [Fact]
        public async Task GetEmployeeAsync_EmployeeIdIsZero_ThrowArgumentException()
        {
            // Arrange
            var employeeId = 0;

            // Act
            var action = async () => await _employeeService.GetEmployeeAsync(employeeId);

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(action);
            Assert.Equal("employeeId", exception.ParamName);
            Assert.Contains("Employee id should be greater than 0.", exception.Message);
        }

        [Fact]
        public async Task GetEmployeeAsync_EmployeeDoesNotExist_ReturnNull()
        {
            // Arrange
            _employeeRepositoryMock
                .Setup(repository => repository.GetEmployeeByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Employee?)null);

            // Act
            var result = await _employeeService.GetEmployeeAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetEmployeeAsync_EmployeeExists_ReturnEmployeeDto()
        {
            // Arrange
            _employeeRepositoryMock
                .Setup(repository => repository.GetEmployeeByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Employee
                {
                    Id = 1,
                    Name = "John",
                    Salary = 123
                });

            // Act
            var result = await _employeeService.GetEmployeeAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result!.Id);
            Assert.Equal("John", result.Name);
            Assert.Equal(123, result.Salary);
        }

        [Fact]
        public async Task SetEmployeeAsync_EmployeeIdIsZero_ThrowArgumentException()
        {
            // Arrange
            var employeeId = 0;

            // Act
            var action = async () => await _employeeService.SetEmployeeAsync(employeeId, "John", 123);

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(action);
            Assert.Equal("employeeId", exception.ParamName);
            Assert.Contains("Employee id should be greater than 0.", exception.Message);
        }

        [Fact]
        public async Task SetEmployeeAsync_EmployeeNameIsEmpty_ThrowArgumentException()
        {
            // Arrange
            var employeeName = " ";

            // Act
            var action = async () => await _employeeService.SetEmployeeAsync(1, employeeName, 123);

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(action);
            Assert.Equal("employeeName", exception.ParamName);
            Assert.Contains("Employee name is required.", exception.Message);
        }

        [Fact]
        public async Task SetEmployeeAsync_EmployeeNameContainsInvalidCharacters_ThrowArgumentException()
        {
            // Arrange
            var employeeName = "John123";

            // Act
            var action = async () => await _employeeService.SetEmployeeAsync(1, employeeName, 123);

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(action);
            Assert.Equal("employeeName", exception.ParamName);
            Assert.Contains("Employee name contains invalid characters.", exception.Message);
        }

        [Fact]
        public async Task SetEmployeeAsync_EmployeeSalaryIsNegative_ThrowArgumentException()
        {
            // Arrange
            var employeeSalary = -1;

            // Act
            var action = async () => await _employeeService.SetEmployeeAsync(1, "John", employeeSalary);

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(action);
            Assert.Equal("employeeSalary", exception.ParamName);
            Assert.Contains("Employee salary can not be negative.", exception.Message);
        }

        [Fact]
        public async Task SetEmployeeAsync_NewEmployee_AddEmployee()
        {
            // Arrange
            Employee? savedEmployee = null;

            _employeeRepositoryMock
                .Setup(repository => repository.GetEmployeeByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Employee?)null);

            _employeeRepositoryMock
                .Setup(repository => repository.AddEmployeeAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
                .Callback<Employee, CancellationToken>((employee, _) => savedEmployee = employee)
                .Returns(Task.CompletedTask);

            // Act
            await _employeeService.SetEmployeeAsync(1, "  John Doe  ", 123);

            // Assert
            Assert.NotNull(savedEmployee);
            Assert.Equal(1, savedEmployee!.Id);
            Assert.Equal("John Doe", savedEmployee.Name);
            Assert.Equal(123, savedEmployee.Salary);
            _employeeRepositoryMock.Verify(repository => repository.UpdateEmployee(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SetEmployeeAsync_ExistingEmployee_UpdateEmployee()
        {
            // Arrange
            var existingEmployee = new Employee
            {
                Id = 1,
                Name = "John",
                Salary = 100
            };

            _employeeRepositoryMock
                .Setup(repository => repository.GetEmployeeByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingEmployee);

            _employeeRepositoryMock
                .Setup(repository => repository.UpdateEmployee(existingEmployee, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _employeeService.SetEmployeeAsync(1, "Jane", 200);

            // Assert
            Assert.Equal("Jane", existingEmployee.Name);
            Assert.Equal(200, existingEmployee.Salary);
            _employeeRepositoryMock.Verify(repository => repository.AddEmployeeAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Never);
            _employeeRepositoryMock.Verify(repository => repository.UpdateEmployee(existingEmployee, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
