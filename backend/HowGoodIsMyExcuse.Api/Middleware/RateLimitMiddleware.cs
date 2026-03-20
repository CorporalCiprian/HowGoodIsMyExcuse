using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.Json;

namespace HowGoodIsMyExcuse.Api.Middleware;

public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly ConcurrentDictionary<string, (int count, DateTime windowStart)> _requestCounts = new();
    private const int MaxRequests = 10;
    private static readonly TimeSpan Window = TimeSpan.FromHours(1);

    public RateLimitMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method == HttpMethods.Post &&
            context.Request.Path.StartsWithSegments("/api/excuses") &&
            !context.Request.Path.Value!.Contains("/vote"))
        {
            var key = GetKey(context);
            var now = DateTime.UtcNow;

            _requestCounts.AddOrUpdate(
                key,
                _ => (1, now),
                (_, existing) =>
                {
                    if (now - existing.windowStart > Window)
                        return (1, now);

                    return (existing.count + 1, existing.windowStart);
                });

            var (count, windowStart) = _requestCounts[key];

            if (count > MaxRequests)
            {
                var retryAfter = (int)(Window - (now - windowStart)).TotalSeconds;

                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.Headers.Append("Retry-After", retryAfter.ToString());
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    error = "Rate limit exceeded. Try again later."
                }));

                return;
            }
        }

        await _next(context);
    }

    private static string GetKey(HttpContext context)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? context.User.FindFirstValue("sub");

        if (!string.IsNullOrEmpty(userId))
            return $"user:{userId}";

        return $"ip:{context.Connection.RemoteIpAddress}";
    }
}