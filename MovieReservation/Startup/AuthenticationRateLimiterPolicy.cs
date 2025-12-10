using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;

namespace MovieReservation.Startup
{
    public class AuthenticationRateLimiterPolicy : IRateLimiterPolicy<string>
    {
        public AuthenticationRateLimiterPolicy()
        {
            OnRejected = async (context, cancellationToken) =>
            {
                string? rateLimiterPolicy = context.HttpContext
                     .GetEndpoint()?.Metadata
                     .GetRequiredMetadata<EnableRateLimitingAttribute>().PolicyName;

                string clientIpAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP Address";

                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<AuthenticationRateLimiterPolicy>>();
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Request IP Address {IpAddress} was rate limited using {Policy} policy.", clientIpAddress, rateLimiterPolicy);
                }
                
                var detailsService = context.HttpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
                ProblemDetailsContext detailsContext = new()
                { 
                    HttpContext = context.HttpContext,
                    ProblemDetails = new ProblemDetails
                    {
                        Title = "Request Rate Limited",
                        Detail = "This request has been rate limited. Please try again later."
                    }
                };

                await detailsService.WriteAsync(detailsContext);
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
