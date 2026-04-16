namespace UVS_Assignment.Cli.Models
{
    public class SetEmployeeCommand
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int EmployeeSalary { get; set; }
    }
}
