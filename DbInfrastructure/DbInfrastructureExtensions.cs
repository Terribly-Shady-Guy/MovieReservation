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
            services.AddScoped((serviceProvider) =>
            {
                List<IDataSeeder> seeders = [
                        new AuthenticationDataSeeder(),
                        new EnumDataSeeder<TheaterType, TheaterTypeLookup>(),
                        new EnumDataSeeder<ReservationStatus, ReservationStatusLookup>()
                    ];

                return new DataSeedingProvider(seeders);
            });

            services.AddDbContext<MovieReservationDbContext>((services, options) =>
            {
                options.UseSqlServer(connectionString, config =>
                {
                    config.EnableRetryOnFailure();
                });

                options.UseSeeding((context, _) =>
                {
                    var seedingProvider = services.GetRequiredService<DataSeedingProvider>();

                    foreach (IDataSeeder seeder in seedingProvider.DataSeeders)
                    {
                        seeder.Add(context);
                    }

                    context.SaveChanges();
                });

                options.UseAsyncSeeding(async (context, _, cancellationToken) =>
                {
                    var provider = services.GetRequiredService<DataSeedingProvider>();

                    foreach (IDataSeeder seeder in provider.DataSeeders)
                    {
                        await seeder.AddAsync(context, cancellationToken);
                    }

                    await context.SaveChangesAsync(cancellationToken);
                });
            });

            return services;
        }
    }
}
