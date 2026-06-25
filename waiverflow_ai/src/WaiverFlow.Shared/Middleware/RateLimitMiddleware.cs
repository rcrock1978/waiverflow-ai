using System.Collections.Concurrent;

namespace WaiverFlow.Shared.Middleware;

public class RateLimitMiddleware
{
    private static readonly ConcurrentDictionary<string, List<DateTime>> RequestLog = new();
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitMiddleware> _log;
    private const int MaxRequests = 100;
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);

    public RateLimitMiddleware(RequestDelegate next, ILogger<RateLimitMiddleware> log)
    {
        _next = next;
        _log = log;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var key = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var now = DateTime.UtcNow;

        var timestamps = RequestLog.GetOrAdd(key, _ => []);
        lock (timestamps)
        {
            timestamps.RemoveAll(t => now - t > Window);
            if (timestamps.Count >= MaxRequests)
            {
                _log.LogWarning("RATE_LIMIT: Exceeded for client {Key}", key);
                context.Response.StatusCode = 429;
                return;
            }
            timestamps.Add(now);
        }

        await _next(context);
    }
}
