using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;

namespace MovieReservation.Startup
{
    public class AuthenticationRateLimiterPolicy : IRateLimiterPolicy<string>
    {
        private const string DefaultIpAddress = "Unknown IP Address";

        public AuthenticationRateLimiterPolicy()
        {
            OnRejected = async (context, cancellationToken) =>
            {
                string? rateLimiterPolicy = context.HttpContext
                     .GetEndpoint()?.Metadata
                     .GetRequiredMetadata<EnableRateLimitingAttribute>().PolicyName;

                string clientIpAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? DefaultIpAddress;

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
                        Detail = "This request has been rate limited. Please try again later."
                    }
                };

                await detailsService.WriteAsync(detailsContext);
            };
        }

        public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected { get; }

        public RateLimitPartition<string> GetPartition(HttpContext httpContext)
        {
            string clientIpAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? DefaultIpAddress;
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
