using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace MovieReservation.Startup
{
    public class AuthenticationRateLimiterPolicy : IRateLimiterPolicy<string>
    {
        public AuthenticationRateLimiterPolicy(ILogger<AuthenticationRateLimiterPolicy> logger)
        {
            OnRejected = async (context, cancellationToken) =>
            {
                string? rateLimiterPolicy = context.HttpContext
                     .GetEndpoint()?.Metadata
                     .GetRequiredMetadata<EnableRateLimitingAttribute>().PolicyName;

                string clientIpAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP Address";

                logger.LogWarning("Request IP Address {IpAddress} was rate limited using {Policy} policy.", clientIpAddress, rateLimiterPolicy);
                await context.HttpContext.Response.WriteAsync("This request has been rate limited. Please try again later.", cancellationToken);
            };
        }

        public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected { get; }

        public RateLimitPartition<string> GetPartition(HttpContext httpContext)
        {
            string clientIpAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP Address";
            return RateLimitPartition.GetSlidingWindowLimiter(clientIpAddress, _ => new SlidingWindowRateLimiterOptions
            {
                Window = TimeSpan.FromMinutes(10),
                PermitLimit = 30,
                AutoReplenishment = true,
                SegmentsPerWindow = 30
            });
        }
    }
}
