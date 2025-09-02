using DbInfrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

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

    internal class TransactionIsolationMiddleware : IMiddleware
    {
        private readonly MovieReservationDbContext _dbContext;

        public TransactionIsolationMiddleware(MovieReservationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            IExecutionStrategy executionStrategy = _dbContext.Database.CreateExecutionStrategy();
            await executionStrategy.ExecuteAsync(async () =>
            {
                using IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();

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
        public const string BypassTransactionIsolation = "X-Test-Isolation-Bypass";
    }
}
