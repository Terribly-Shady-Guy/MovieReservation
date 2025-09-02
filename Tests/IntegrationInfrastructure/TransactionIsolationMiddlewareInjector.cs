using DbInfrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.IntegrationInfrastructure
{
    internal class TransactionIsolationMiddlewareInjector : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return (app) =>
            {
                app.UseMiddleware<TransactionIsolationMiddleware>();
                next(app);
            };
        }
    }

    public class TransactionIsolationMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            MovieReservationDbContext dbContext = context.RequestServices.GetRequiredService<MovieReservationDbContext>();

            IExecutionStrategy executionStrategy = dbContext.Database.CreateExecutionStrategy();
            await executionStrategy.ExecuteAsync(async () =>
            {
                using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

                await next(context);

                if (ShouldBypassTransactionIsolation(context))
                {
                    await transaction.CommitAsync();
                }
                else
                {
                    await transaction.RollbackAsync();
                }
            });
        }

        private static bool ShouldBypassTransactionIsolation(HttpContext context)
        {
            return context.Request.Headers.TryGetValue(IntegrationTestCustomHeaders.BypassTransactionIsolation, out var value)
               && bool.TryParse(value, out bool result)
               && result;
        }
    }

    internal static class IntegrationTestCustomHeaders
    {
        public const string BypassTransactionIsolation = "X-Transaction-Bypass";
    }
}
