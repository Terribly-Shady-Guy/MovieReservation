using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;

namespace MovieReservation.Startup
{
    public class AuthenticationRateLimiterPolicy : IRateLimiterPolicy<string>
    {
        public AuthenticationRateLimiterPolicy()
        {
            OnRejected = async (rejectedContext, cancellationToken) =>
            {
                Endpoint? rateLimitedEndpoint = rejectedContext.HttpContext.GetEndpoint();
                string? rateLimiterPolicy = rateLimitedEndpoint?.Metadata.GetRequiredMetadata<EnableRateLimitingAttribute>().PolicyName;

                string clientIpAddress = GetClientIpAddress(rejectedContext.HttpContext);

                var logger = rejectedContext.HttpContext.RequestServices.GetRequiredService<ILogger<AuthenticationRateLimiterPolicy>>();
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Request IP Address {IpAddress} was rate limited using {PolicyName} policy for {EndpointName}. Request trace identifier: {TraceIdentifier}",
                                      clientIpAddress,
                                      rateLimiterPolicy,
                                      rateLimitedEndpoint?.DisplayName,
                                      rejectedContext.HttpContext.TraceIdentifier);
                }
                
                var detailsService = rejectedContext.HttpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
                ProblemDetailsContext detailsContext = new()
                { 
                    HttpContext = rejectedContext.HttpContext,
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
            string clientIpAddress = GetClientIpAddress(httpContext);
            return RateLimitPartition.GetSlidingWindowLimiter(clientIpAddress, _ => new SlidingWindowRateLimiterOptions
            {
                Window = TimeSpan.FromMinutes(10),
                PermitLimit = 30,
                SegmentsPerWindow = 30
            });
        }

        private static string GetClientIpAddress(HttpContext httpContext)
        {
            const string DefaultIpAddress = "Unknown IP Address";
            return httpContext.Connection?.RemoteIpAddress?.ToString() ?? DefaultIpAddress;
        }
    }
}
