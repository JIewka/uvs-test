using Microsoft.EntityFrameworkCore;
using UVS_Assignment.Entities;

namespace UVS_Assignment.Infrastructure
{
    public class EmployeeDbContext : DbContext, IEmployeeDbContext
    {
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options)
        {

        }

        public DbSet<Employee> Employees => Set<Employee>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("employees");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("employeeid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasColumnName("employeename")
                    .HasMaxLength(128)
                    .IsRequired();

                entity.Property(e => e.Salary)
                    .HasColumnName("employeesalary")
                    .IsRequired();
            });
        }
    }
}
