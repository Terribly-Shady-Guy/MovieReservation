using DbInfrastructure.DataSeeding;
using DbInfrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


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
            services.AddScoped<IDataSeeder, AuthenticationDataSeeder>();
            services.AddScoped<IDataSeeder, EnumDataSeeder<TheaterType, TheaterTypeLookup>>();
            services.AddScoped<IDataSeeder, EnumDataSeeder<ReservationStatus, ReservationStatusLookup>>();

            services.AddScoped<IDataSeedingProvider>(static (serviceProvider) =>
            {
                ILogger<IDataSeedingProvider> logger = serviceProvider.GetRequiredService<ILogger<IDataSeedingProvider>>();

                List<IDataSeeder> seeders = serviceProvider.GetServices<IDataSeeder>()
                    .ToList();

                logger.LogInformation("Found {SeederCount} data seeders.", seeders.Count);
                return new DataSeedingProvider(seeders);
            });
            
            services.AddDbContext<MovieReservationDbContext>((serviceProvider, options) =>
            {
                options.UseSqlServer(connectionString, config =>
                {
                    config.EnableRetryOnFailure();
                });

                options.UseSeeding((context, _) =>
                {
                    var seedingProvider = serviceProvider.GetRequiredService<IDataSeedingProvider>();
                    seedingProvider.Seed(context);

                    context.SaveChanges();
                });

                options.UseAsyncSeeding(async (context, _, cancellationToken) =>
                {
                    var seedingProvider = serviceProvider.GetRequiredService<IDataSeedingProvider>();
                    await seedingProvider.SeedAsync(context, cancellationToken);

                    await context.SaveChangesAsync(cancellationToken);
                });
            });

            return services;
        }
    }
}
