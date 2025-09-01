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
                app.Use(static async (HttpContext context, RequestDelegate next) =>
                {
                    MovieReservationDbContext dbContext = context.RequestServices.GetRequiredService<MovieReservationDbContext>();

                    IExecutionStrategy executionStrategy = dbContext.Database.CreateExecutionStrategy();

                    await executionStrategy.ExecuteAsync(async () =>
                    {
                        using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

                        await next(context);
                        await transaction.RollbackAsync();
                    });
                });

                next(app);
            };
        }
    }
}
