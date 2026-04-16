$Env:UvsTaskPassword = "guest"
$Env:UvsTaskDatabase = "uvsproject"
$Env:UvsTaskPort = "7777"
$schemaPath = Join-Path $PSScriptRoot "dbSchema.sql"

Write-Host ""
Write-Host "This script will set up a postgres database hosted in a docker container"
Read-Host "Press ENTER to continue"

docker pull postgres
if (-not $?) { exit 1 }

$portAssign = "$($Env:UvsTaskPort):5432"
$container = docker run -e "POSTGRES_PASSWORD=$Env:UvsTaskPassword" -p $portAssign -d postgres
if (-not $?) { exit 1 }

try {
    Write-Host "Database starting. Waiting for postgres to become ready..."

    $databaseReady = $false

    for ($attempt = 0; $attempt -lt 30; $attempt++) {
        Start-Sleep -Seconds 2
        docker exec $container pg_isready -U postgres | Out-Null

        if ($LASTEXITCODE -eq 0) {
            $databaseReady = $true
            break
        }
    }

    if (-not $databaseReady) {
        throw "Postgres did not become ready in time."
    }

    Write-Host "Creating database $Env:UvsTaskDatabase"
    docker exec $container psql -v ON_ERROR_STOP=1 -U postgres -c "CREATE DATABASE $Env:UvsTaskDatabase;" | Out-Null
    if ($LASTEXITCODE -ne 0) { exit 1 }

    Write-Host "Applying schema from $schemaPath"
    Get-Content -Raw $schemaPath | docker exec -i $container psql -v ON_ERROR_STOP=1 -U postgres -d $Env:UvsTaskDatabase
    if ($LASTEXITCODE -ne 0) { exit 1 }

    Write-Host ""
    Write-Host "The database is ready to use" -ForegroundColor Green
    Write-Host "Connection string: 'Host=localhost;Port=$Env:UvsTaskPort;Database=$Env:UvsTaskDatabase;Username=postgres;Password=$Env:UvsTaskPassword'"
    Write-Host "Schema applied to database:"
    Get-Content $schemaPath
    Write-Host ""
    Write-Host "Press Ctrl+C to stop the database server and exit" -ForegroundColor Green
    docker attach $container
}
finally {
    docker stop $container | Out-Null
}
