# UVS Assignment

## Project Structure

- `UVS_Assignment/` - console application
- `UVS_Assignment.Tests/` - unit tests
- `UVS_Assignment/Scripts/` - helper scripts and SQL schema

## Database setup

Run this script to start PostgreSQL in Docker, create the `uvsproject` database
and apply the dbSchema: 

```powershell
cd .\UVS_Assignment\Scripts
.\setUpDatabase.ps1
```

## Testing the Application

Run this script to test application:

```powershell
cd .\UVS_Assignment\Scripts
.\verifySubmission.ps1
```

## Test the Application manually

Working directory: `.\UVS_Assignment`

Run this command to set employee:

```powershell
dotnet run --no-build -- set-employee --employeeId 1 --employeeName John --employeeSalary 123
```

Run this command to get employee:

```powershell
dotnet run --no-build -- get-employee --employeeId 1
```

## Remarks

The database schema was adjusted slightly from the original version. A primary key constraint was added to
`employeeId`, while the application still requires the caller to provide the employeeId explicitly.
This keeps the command contract unchanged and enforces uniqueness at the database level.

The PowerShell scripts were adjusted slightly to match the current directory structure and setup flow.
