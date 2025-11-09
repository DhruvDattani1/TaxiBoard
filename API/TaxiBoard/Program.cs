using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using TaxiBoard.Data;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

//get rid of the logger and the Userserilog to go back to regular logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console(
        theme: Serilog.Sinks.SystemConsole.Themes.SystemConsoleTheme.Colored,
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
    )   
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .CreateLogger();

builder.Host.UseSerilog();

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

// app.MapGet("/", () => Results.Ok("TaxiBoard API is running"));

//no serilog
// using (var scope = app.Services.CreateScope())
// {
//     var dbContext = scope.ServiceProvider.GetRequiredService<TaxiBoard.Data.TaxiBoardContext>();
//     try
//     {
//         var conn = dbContext.Database.GetDbConnection();
//         conn.Open();
//         Console.WriteLine("Connected to Postgres successfully!");
//         conn.Close();
//     }
//     catch (Exception ex)
//     {
//         Console.WriteLine($"Failed to connect to Postgres: {ex.Message}");
//     }
// }

//test the connection
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TaxiBoard.Data.TaxiBoardContext>();
    try
    {
        var conn = dbContext.Database.GetDbConnection();
        conn.Open();
        Log.Information("Connected to Postgres successfully");
        conn.Close();
    }
    catch (Exception ex)
    {
        Log.Error(ex, $"Failed to connect to Postgres: {ex.Message}");
    }
}

// try a query. After models are defined I'll be able to register them and execute LINQ queries
// using (var scope = app.Services.CreateScope())
// {
//     var dbContext = scope.ServiceProvider.GetRequiredService<TaxiBoard.Data.TaxiBoardContext>();
//     try
//     {
//         var conn = dbContext.Database.GetDbConnection();
//         conn.Open();

//         using (var cmd = conn.CreateCommand())
//         {
//             cmd.CommandText = "SELECT COUNT(*) FROM yellow_tripdata;";
//             var result = cmd.ExecuteScalar();
//             Console.WriteLine($"Trip count from DB: {result}");
//         }

//         conn.Close();
//     }
//     catch (Exception ex)
//     {
//         Console.WriteLine($"Query failed: {ex.Message}");
//     }
// }


var appTask = Task.Run(async () =>
{
    await Task.Delay(2000); 

    using var client = new HttpClient { BaseAddress = new Uri("http://localhost:5034") };

    async Task TestEndpoint(string path, string name, int maxLen = 300)
    {
        try
        {
            var response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var preview = json.Substring(0, Math.Min(maxLen, json.Length)) + "...";

                Log.Information("{Endpoint} responded successfully (Length={Length})", name, json.Length);
                Log.Information("{Endpoint} response preview: {Preview}", name, preview);
            }
            else
            {
                Log.Warning("{Endpoint} returned status: {StatusCode}", name, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to query {Endpoint}", name);
        }
    }



    await TestEndpoint("/api/trips?page=1&pageSize=1", "/api/trips");
    await TestEndpoint("/api/vendors", "/api/vendors");
    await TestEndpoint("/api/paymenttypes", "/api/paymenttypes");
    await TestEndpoint("/api/zones", "/api/zones");

    const string startDate = "2025-01-15";
    const string endDate = "2025-01-17";
    await TestEndpoint($"/api/analytics?startDate={startDate}&endDate={endDate}", "/api/analytics");
});


// no serilog
// var appTask = Task.Run(async () =>
// {
//     await Task.Delay(2000); 

//     using var client = new HttpClient { BaseAddress = new Uri("http://localhost:5034") };

//     async Task TestEndpoint(string path, string name, int maxLen = 300)
//     {
//         try
//         {
//             var response = await client.GetAsync(path);
//             if (response.IsSuccessStatusCode)
//             {
//                 var json = await response.Content.ReadAsStringAsync();
//                 Console.WriteLine($"{name} endpoint responded successfully:");
//                 Console.WriteLine(json.Substring(0, Math.Min(maxLen, json.Length)) + "...\n");
//             }
//             else
//             {
//                 Console.WriteLine($"{name} returned status: {response.StatusCode}");
//             }
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Failed to query {name}: {ex.Message}");
//         }
//     }

//     await TestEndpoint("/api/trips?page=1&pageSize=1", "/api/trips");
//     await TestEndpoint("/api/vendors", "/api/vendors");
//     await TestEndpoint("/api/paymenttypes", "/api/paymenttypes");
//     await TestEndpoint("/api/zones", "/api/zones");

//     const string startDate = "2025-01-15";
//     const string endDate = "2025-01-17";
//     await TestEndpoint($"/api/analytics?startDate={startDate}&endDate={endDate}", "/api/analytics");
// });

app.Run();
await appTask;


