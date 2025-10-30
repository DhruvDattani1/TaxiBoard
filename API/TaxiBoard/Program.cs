using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using TaxiBoard.Data;

var builder = WebApplication.CreateBuilder(args);

// DotNetEnv.Env.Load();

// var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
//     ?? builder.Configuration.GetConnectionString("DefaultConnection");

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// So logging is enabled when your loading from appsettings as opposed to env

builder.Services.AddDbContext<TaxiBoardContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

//test the connection
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TaxiBoard.Data.TaxiBoardContext>();
    try
    {
        var conn = dbContext.Database.GetDbConnection();
        conn.Open();
        Console.WriteLine("Connected to Postgres successfully!");
        conn.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to connect to Postgres: {ex.Message}");
    }
}

//try a query. After models are defined I'll be able to register them and execute LINQ queries
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TaxiBoard.Data.TaxiBoardContext>();
    try
    {
        var conn = dbContext.Database.GetDbConnection();
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "SELECT COUNT(*) FROM yellow_tripdata;";
            var result = cmd.ExecuteScalar();
            Console.WriteLine($"Trip count from DB: {result}");
        }

        conn.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Query failed: {ex.Message}");
    }
}


app.MapGet("/", () => Results.Ok("TaxiBoard is running"));

app.Run();


