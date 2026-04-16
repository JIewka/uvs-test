using Microsoft.EntityFrameworkCore;
using UVS_Assignment.Entities;

namespace UVS_Assignment.Infrastructure
{
    public interface IEmployeeDbContext
    {
        DbSet<Employee> Employees { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
