using DbInfrastructure.DataSeeding;
using DbInfrastructure.Models;
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
            services.AddDbContext<MovieReservationDbContext>((serviceProvider, options) =>
            {
                options.UseSqlServer(connectionString, config =>
                {
                    config.EnableRetryOnFailure();
                });

                IDataSeeder[] dataSeeders = [
                        new AuthenticationDataSeeder(),
                        new EnumDataSeeder<TheaterType, TheaterTypeLookup>(),
                        new EnumDataSeeder<ReservationStatus, ReservationStatusLookup>()
                    ];

                options.UseSeeding((context, _) =>
                {
                    foreach (var helper in dataSeeders)
                    {
                        helper.Add(context);
                    }

                    context.SaveChanges();
                });

                options.UseAsyncSeeding(async (context, _, cancellationToken) =>
                {
                    foreach(var helper in dataSeeders)
                    {
                        await helper.AddAsync(context, cancellationToken);
                    }

                    await context.SaveChangesAsync(cancellationToken);
                });
            });

            return services;
        }
    }
}
