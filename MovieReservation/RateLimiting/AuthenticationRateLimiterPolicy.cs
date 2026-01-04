using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;

namespace MovieReservation.RateLimiting
{
    public class AuthenticationRateLimiterPolicy : IRateLimiterPolicy<string>
    {
        public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected { get; } = HandleRejected;

        public RateLimitPartition<string> GetPartition(HttpContext httpContext)
        {
            string clientIpAddress = GetClientIpAddress(httpContext);
            return RateLimitPartition.GetSlidingWindowLimiter(clientIpAddress, _ => new SlidingWindowRateLimiterOptions
            {
                Window = TimeSpan.FromMinutes(10),
                PermitLimit = 30,
                SegmentsPerWindow = 10
            });
        }

        private static async ValueTask HandleRejected(OnRejectedContext rejectedContext, CancellationToken cancellationToken)
        {
            Endpoint? rateLimitedEndpoint = rejectedContext.HttpContext.GetEndpoint();
            var rateLimitingAttribute = rateLimitedEndpoint?.Metadata.GetRequiredMetadata<EnableRateLimitingAttribute>();

            string clientIpAddress = GetClientIpAddress(rejectedContext.HttpContext);

            var logger = rejectedContext.HttpContext.RequestServices.GetRequiredService<ILogger<AuthenticationRateLimiterPolicy>>();
            logger.AuthenticationRequestRateLimited(
                clientIpAddress,
                rateLimitingAttribute?.PolicyName ?? "Unknown policy",
                rateLimitedEndpoint?.DisplayName ?? "Unknown endpoint",
                rejectedContext.HttpContext.TraceIdentifier);

            if (rejectedContext.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfterTime))
            {
                rejectedContext.HttpContext.Response.Headers.RetryAfter = ((int)retryAfterTime.TotalSeconds).ToString();
            }

            ProblemDetailsContext detailsContext = new()
            {
                HttpContext = rejectedContext.HttpContext,
                ProblemDetails = new ProblemDetails
                {
                    Detail = "This request has been rate limited. Please try again later."
                }
            };

            var detailsService = rejectedContext.HttpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
            await detailsService.WriteAsync(detailsContext);
        }

        private static string GetClientIpAddress(HttpContext httpContext)
        {
            const string DefaultIpAddress = "Unknown IP Address";
            return httpContext.Connection?.RemoteIpAddress?.ToString() ?? DefaultIpAddress;
        }
    }
}
