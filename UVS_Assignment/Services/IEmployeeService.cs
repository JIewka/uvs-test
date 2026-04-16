using UVS_Assignment.Dtos;

namespace UVS_Assignment.Services
{
    public interface IEmployeeService
    {
        Task<EmployeeDto?> GetEmployeeAsync(int employeeId, CancellationToken cancellationToken = default);
        Task SetEmployeeAsync(int employeeId, string employeeName, int employeeSalary, CancellationToken cancellationToken = default);
    }
}
