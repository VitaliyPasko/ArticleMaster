using ArticleMaster.API.Middlewares;
using ArticleMaster.Application;
using ArticleMaster.Persistence;
using AspNetCore.Serilog.RequestLoggingMiddleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
    .AddEnvironmentVariables();

// Log.Logger = new LoggerConfiguration()
//     .CreateLogger();

builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddSwaggerGen();


var app = builder.Build();
// app.UseSerilogRequestLogging();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCustomExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.MapControllers();
app.Run();
public partial class Program{}