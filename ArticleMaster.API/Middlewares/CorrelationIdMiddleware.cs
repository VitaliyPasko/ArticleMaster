using Serilog.Context;

namespace ArticleMaster.API.Middlewares;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // Генерируйте Correlation ID (обычно это GUID)
        var correlationId = Guid.NewGuid().ToString();

        // Добавьте Correlation ID в контекст логгера
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}