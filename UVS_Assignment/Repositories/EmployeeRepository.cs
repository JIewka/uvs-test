using Microsoft.EntityFrameworkCore;
using UVS_Assignment.Entities;
using UVS_Assignment.Infrastructure;

namespace UVS_Assignment.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IEmployeeDbContext _context;

        public EmployeeRepository(IEmployeeDbContext context)
        {
            _context = context;
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int employeeId, CancellationToken cancellationToken = default)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken);
        }

        public async Task AddEmployeeAsync(Employee employee, CancellationToken cancellationToken = default)
        {
            await _context.Employees.AddAsync(employee, cancellationToken);
            await SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateEmployee(Employee employee, CancellationToken cancellationToken = default)
        {
            _context.Employees.Update(employee);
            await SaveChangesAsync(cancellationToken);
        }

        private async Task  SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
