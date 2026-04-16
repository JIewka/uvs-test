using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UVS_Assignment.Cli;
using UVS_Assignment.Infrastructure;
using UVS_Assignment.Repositories;
using UVS_Assignment.Services;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection");

var services = new ServiceCollection();

services.AddDbContext<EmployeeDbContext>(options =>
    options.UseNpgsql(connectionString));

services.AddScoped<IEmployeeDbContext>(provider =>
    provider.GetRequiredService<EmployeeDbContext>());

services.AddScoped<IEmployeeRepository, EmployeeRepository>();
services.AddScoped<IEmployeeService, EmployeeService>();
services.AddScoped<ICommandParser, CommandParser>();
services.AddScoped<ICommandExecutor, CommandExecutor>();

await using var serviceProvider = services.BuildServiceProvider();
await using var scope = serviceProvider.CreateAsyncScope();

try
{
    var commandExecutor = scope.ServiceProvider.GetRequiredService<ICommandExecutor>();
    await commandExecutor.ExecuteAsync(args);
}
catch (Exception ex)
{
    Console.Error.WriteLine("Error occurred:");
    Console.Error.WriteLine(ex.Message);
    Environment.ExitCode = 1;
}
