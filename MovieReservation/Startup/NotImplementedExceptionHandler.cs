using Microsoft.AspNetCore.Diagnostics;

namespace MovieReservation.Startup
{
    public class NotImplementedExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;

        public NotImplementedExceptionHandler(IProblemDetailsService detailsService)
        {
            _problemDetailsService = detailsService;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not NotImplementedException)
            {
                return false;
            }

            httpContext.Response.StatusCode = StatusCodes.Status501NotImplemented;

            var context = new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails()
                {
                    Title = "Endpoint Not Implemented",
                    Detail = "This endpoint is currently not implemented and will be in the future."
                },
                Exception = exception
            };

            return await _problemDetailsService.TryWriteAsync(context);
        }
    }
}
