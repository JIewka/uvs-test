using System.Text.RegularExpressions;
using UVS_Assignment.Dtos;
using UVS_Assignment.Entities;
using UVS_Assignment.Repositories;

namespace UVS_Assignment.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        private static readonly Regex EmployeeNameRegex = new(@"^[\p{L}\p{M}\s'\-]+$", RegexOptions.Compiled);

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<EmployeeDto?> GetEmployeeAsync(int employeeId, CancellationToken cancellationToken = default)
        {
            ValidateEmployeeId(employeeId);

            var employee = await _employeeRepository.GetEmployeeByIdAsync(employeeId, cancellationToken);

            if (employee is null)
                return null;

            return new EmployeeDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Salary = employee.Salary
            };
        }

        public async Task SetEmployeeAsync(
            int employeeId,
            string employeeName,
            int employeeSalary,
            CancellationToken cancellationToken = default
        )
        {
            employeeName = ValidateAndNormalizeEmployee(employeeId, employeeName, employeeSalary);

            var existingEmployee = await _employeeRepository.GetEmployeeByIdAsync(employeeId, cancellationToken);

            if (existingEmployee is null)
            {
                var newEmployee = new Employee
                {
                    Id = employeeId,
                    Name = employeeName,
                    Salary = employeeSalary
                };

                await _employeeRepository.AddEmployeeAsync(newEmployee, cancellationToken);
            }
            else
            {
                existingEmployee.Name = employeeName;
                existingEmployee.Salary = employeeSalary;

                await _employeeRepository.UpdateEmployee(existingEmployee, cancellationToken);
            }
        }

        private static void ValidateEmployeeId(int employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException("Employee id should be greater than 0.", nameof(employeeId));
        }

        private static string ValidateAndNormalizeEmployee(int employeeId, string employeeName, int employeeSalary)
        {
            ValidateEmployeeId(employeeId);

            if (string.IsNullOrWhiteSpace(employeeName))
                throw new ArgumentException("Employee name is required.", nameof(employeeName));

            employeeName = employeeName.Trim();

            if (employeeName.Length > 128)
                throw new ArgumentException("Employee name can not be longer than 128 chars.", nameof(employeeName));

            if (!EmployeeNameRegex.IsMatch(employeeName))
                throw new ArgumentException("Employee name contains invalid characters.", nameof(employeeName));

            if (employeeSalary < 0)
                throw new ArgumentException("Employee salary can not be negative.", nameof(employeeSalary));

            return employeeName;
        }
    }
}
