using System.ComponentModel.DataAnnotations;
using big_agi_syncserver;
using big_agi_syncserver.Data;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.AddEnvironmentVariables();

// Read configuration from appsettings.json.
builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true);

var connectionString = builder.Configuration.GetValue<string>("Database:ConnectionString");
if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("Database connection string is not set in appsettings.json or environment variable");
}

var connectionStringType = builder.Configuration.GetValue<string>("Database:ConnectionStringType");
if (string.IsNullOrEmpty(connectionStringType))
    throw new Exception("Database connection string type is not set in appsettings.json or environment variable");

// Use EF Core context ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    switch (connectionStringType.ToLower())
    {
        case "sqlite":
            options.UseSqlite(connectionString);
            break;
        case "mssql":
            options.UseSqlServer(connectionString);
            break;

        default:
            throw new Exception($"Database connection string type is not set to a Known value '{connectionStringType}'");
    }
});

// Configure JSON deserialization options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


#region Minimal API

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";

        var exceptionObject = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (exceptionObject != null)
        {
            if (exceptionObject is ValidationException validationException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = validationException.Message
                });
            }
            else if (exceptionObject is DbUpdateException dbUpdateException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { error = dbUpdateException.InnerException?.Message ?? dbUpdateException.Message });
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { error = exceptionObject.Message });
            }
        }
    });
});

app.MapGet("/getLastSync/{syncKey}", Endpoints.GetLastSync).WithName("GetLastSync");

app.MapPost("/fullSync/{syncKey}", Endpoints.FullSync).WithName("FullSync");
// app.MapPost("/fullSync/{syncKey}", (Endpoints.FullSyncDto syncData, string syncKey, ApplicationDbContext context, CancellationToken cancellationToken) 
//     => Endpoints.FullSync).WithName("FullSync");

#endregion


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}