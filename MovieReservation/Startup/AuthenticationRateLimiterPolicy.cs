using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace MovieReservation.Startup
{
    public class AuthenticationRateLimiterPolicy : IRateLimiterPolicy<string>
    {
        public AuthenticationRateLimiterPolicy(ILogger<AuthenticationRateLimiterPolicy> logger)
        {
            OnRejected = (context, cancellationToken) =>
            {
                string? rateLimiterPolicy = context.HttpContext
                    .GetEndpoint()?.Metadata
                    .GetRequiredMetadata<EnableRateLimitingAttribute>().PolicyName;

                var clientIpAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();

                logger.LogWarning("Request IP Address {IpAddress} was rate limited using {Policy} policy.", clientIpAddress, rateLimiterPolicy);
                return ValueTask.CompletedTask;
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
            });
        }
    }
}
