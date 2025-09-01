using DbInfrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Integration
{
    internal class TransactionIsolationMiddlewareInjector : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return (IApplicationBuilder app) =>
            {
                app.Use(static async (HttpContext context, RequestDelegate next) =>
                {
                    using MovieReservationDbContext dbContext = context.RequestServices.GetRequiredService<MovieReservationDbContext>();

                    using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

                    try
                    {
                        await next(context);
                        await transaction.RollbackAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                });

                next(app);
            };
        }
    }
}
