namespace MovieReservation.RateLimiting
{
    public static partial class RateLimitingLoggerExtensions
    {
        [LoggerMessage(
            level: LogLevel.Warning, 
            message: "Request from IP address {ClientIpAddress} was rate limited by {PolicyName} policy for endpoint {EndpointName}. Request trace identifier: {TraceIdentifier}")]
        public static partial void AuthenticationRequestRateLimited(
            this ILogger<AuthenticationRateLimiterPolicy> logger,
            string clientIpAddress,
            string policyName,
            string endpointName,
            string traceIdentifier);
    }
}
