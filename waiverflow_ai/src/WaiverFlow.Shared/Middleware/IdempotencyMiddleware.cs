using System.Collections.Concurrent;

namespace WaiverFlow.Shared.Middleware;

public class IdempotencyMiddleware
{
    private static readonly ConcurrentDictionary<string, bool> ProcessedKeys = new();
    private readonly RequestDelegate _next;
    private readonly ILogger<IdempotencyMiddleware> _log;

    public IdempotencyMiddleware(RequestDelegate next, ILogger<IdempotencyMiddleware> log)
    {
        _next = next;
        _log = log;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method is "POST" or "PUT" or "PATCH")
        {
            var key = context.Request.Headers["Idempotency-Key"].FirstOrDefault();
            if (key is not null)
            {
                if (!ProcessedKeys.TryAdd(key, true))
                {
                    _log.LogWarning("IDEMPOTENCY: Duplicate key {Key} for {Method} {Path}", key, context.Request.Method, context.Request.Path);
                    context.Response.StatusCode = 409;
                    context.Response.Headers["Idempotency-Key-Conflict"] = key;
                    return;
                }
                _log.LogInformation("IDEMPOTENCY: Key {Key} registered for {Method} {Path}", key, context.Request.Method, context.Request.Path);
            }
        }

        await _next(context);
    }
}
