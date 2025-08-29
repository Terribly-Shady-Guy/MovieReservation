using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace DbInfrastructure
{
    public static class DbInfrastructureExtensions
    {
        /// <summary>
        /// Adds <see cref="MovieReservationDbContext" /> and configures ef core using the provided connection string.
        /// </summary>
        /// <param name="services">The service collection instance used by the app.</param>
        /// <param name="connectionString">The connection string for SQL Server</param>
        /// <returns>The same service collection passed as a parameter</returns>
        public static IServiceCollection AddDbInfrastructure(this IServiceCollection services, string? connectionString)
        {
            services.AddDbContext<MovieReservationDbContext>(options =>
            {
                options.UseSqlServer(connectionString, config =>
                {
                    config.EnableRetryOnFailure();
                });

                options.UseSeeding((context, _) =>
                {
                    var helper = new DataSeedingHelper(context);
                    helper.AddRootUser();
                    helper.AddEnums();

                    context.SaveChanges();
                });

                options.UseAsyncSeeding(async (context, _, cancellationToken) =>
                {
                    var helper = new DataSeedingHelper(context);
                    await helper.AddRootUserAsync(cancellationToken);
                    await helper.AddEnumsAsync(cancellationToken);

                    await context.SaveChangesAsync(cancellationToken);
                });
            });

            return services;
        }
    }
}
