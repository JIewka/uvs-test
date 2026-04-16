using UVS_Assignment.Entities;

namespace UVS_Assignment.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetEmployeeByIdAsync(int employeeId, CancellationToken cancellationToken = default);
        Task AddEmployeeAsync(Employee employee, CancellationToken cancellationToken = default);
        Task UpdateEmployee(Employee employee, CancellationToken cancellationToken = default);
    }
}