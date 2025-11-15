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
- dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

## Swagger UI package
- dotnet add package Swashbuckle.AspNetCore

## Local Caching package
- dotnet add package Microsoft.Extensions.Caching.Memory


## Serilog package for logging
- dotnet add package Serilog.AspNetCore
- dotnet add package Serilog.Sinks.Console
- dotnet add package Serilog.Sinks.File
- dotnet add package Serilog.Sinks.Seq

# Env setup
- touch .env
- ASPNETCORE_ENVIRONMENT=Development
- DB_CONNECTION_STRING=Host=localhost;Database=nyc_yellow_taxi_db;Username=yellow_taxi_user;Password=yellow_taxi_user
- dotnet add package DotNetEnv
- touch appsettings.Development.json

# EF Core setup
- mkdir Data (in TaxiBoard root)
- Data/TaxiContext.cs (this is the session built between the app and postgres)
- set DB_CONNECTION_STRING if its not already (env)

# Models
- dotnet ef migrations add InitTaxiModels
    - compares model classes against db
    - generate new migration file under /Migrations
- dotnet ef database update (I'm not doing this since I have the db setup already)
    - Applies said migration to DB
    - in the future it is best to do the follwoing if you want to add incremental changes
        - dotnet ef migrations add AddNewFeature
        - dotnet ef database update

# Add Surrogate id key to db

- ALTER TABLE yellow_tripdata ADD COLUMN id SERIAL PRIMARY KEY;

# Setup Testing project

- dotnet new xunit -o TaxiBoard.Tests
- dotnet new sln -n TaxiBoard  
- dotnet sln TaxiBoard.sln add TaxiBoard/TaxiBoard.csproj
- dotnet sln TaxiBoard.sln add TaxiBoard.Tests/TaxiBoard.Tests.csproj

- cd TaxiBoard.Tests
- dotnet add reference ../TaxiBoard/TaxiBoard.csproj


## Required NuGet packages

- dotnet add package Microsoft.NET.Test.Sdk --version 17.12.0
- dotnet add package xunit --version 2.9.2
- dotnet add package xunit.runner.visualstudio --version 2.8.2
- dotnet add package Microsoft.EntityFrameworkCore.InMemory --version 9.0.0
- dotnet add package coverlet.collector --version 6.0.2













