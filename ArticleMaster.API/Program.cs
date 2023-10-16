using ArticleMaster.API.Middlewares;
using ArticleMaster.Application;
using ArticleMaster.Persistence;
using AspNetCore.Serilog.RequestLoggingMiddleware;
using Serilog;
using CorrelationIdMiddleware = ArticleMaster.API.Middlewares.CorrelationIdMiddleware;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
    .AddEnvironmentVariables();


var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .Enrich.WithCorrelationId()
    .WriteTo.Console(outputTemplate:"[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level}] [C: {CorrelationId}, R:{RequestId}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();
builder.Logging.AddSerilog(logger);
  
builder.Services.AddSingleton<Serilog.ILogger>(logger);
builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSerilogRequestLogging();
app.UseMiddleware<CorrelationIdMiddleware>();
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