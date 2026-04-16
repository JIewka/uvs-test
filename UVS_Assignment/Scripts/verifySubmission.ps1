
$projectRoot = Split-Path -Path $PSScriptRoot -Parent
Set-Location $projectRoot

$Env:ConnectionStrings__DefaultConnection="Host=localhost;Port=7777;Database=uvsproject;Username=postgres;Password=guest"
dotnet build

dotnet run --no-build set-employee --employeeId 1 --employeeName John --employeeSalary 123
dotnet run --no-build set-employee --employeeId 2 --employeeName Steve --employeeSalary 456

dotnet run --no-build get-employee --employeeId 1
dotnet run --no-build get-employee --employeeId 2
