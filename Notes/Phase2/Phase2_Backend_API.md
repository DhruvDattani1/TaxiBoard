# Install .NET SDK Latest LTS (9.0)
 - sudo dnf install dotnet-sdk-9.0

# Install Entity Framework Core CLI (This is your ORM)
- dotnet too install --global dotnet-ef
- export PATH="$PATH:$HOME/.dotnet/tools"

## Test with:
- dotnet ef

# Scaffold a project & run
- dotnet new webapi -n TaxiBoard
- dotnet run 

## EF Core + Npgsql Provider install in project dir
- dotnet add package Microsoft.EntityFrameworkCore
- dotnet add package Microsoft.EntityFrameworkCore.Design
- dotnet add package Npgsql.EntityFraemworkCore.Postgresql

## Swagger UI package
- dotnet add package Swashbuckle.AspNetCore

## Local Caching package
- dotnet add package Microsoft.Extensions.Caching.Memory

## Serilog package for logging
- dotnet add package Serilog.AspNetCore

# Env setup
- touch .env
- ASPNETCORE_ENVIRONMENT=Development
- ConnectionStrings__Default=Host=localhost;Database=nyc_yellow_taxi_db
Username=yellow_taxi_user;Password=yellow_taxi_user
- dotnet add package DotNetEnv
- touch appsettings.Development.json

# EF Core setup
- 





