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

                options.UseSeeding((context, _) =>
                {
                    var authHelper = new AuthenticationSeedingHelper(context);
                    authHelper.Add();
                    
                    var reservationStatusHelper = new EnumSeedingHelper<ReservationStatus, ReservationStatusLookup>(context);
                    reservationStatusHelper.Add();

                    var theaterTypesHelper = new EnumSeedingHelper<TheaterType, TheaterTypeLookup>(context);
                    theaterTypesHelper.Add();

                    context.SaveChanges();
                });

                options.UseAsyncSeeding(async (context, _, cancellationToken) =>
                {
                    var authHelper = new AuthenticationSeedingHelper(context);
                    await authHelper.AddAsync(cancellationToken);

                    var reservationStatusHelper = new EnumSeedingHelper<ReservationStatus, ReservationStatusLookup>(context);
                    await reservationStatusHelper.AddAsync(cancellationToken);

                    var theaterTypeHelper = new EnumSeedingHelper<TheaterType, TheaterTypeLookup>(context);
                    await theaterTypeHelper.AddAsync(cancellationToken);

                    await context.SaveChangesAsync(cancellationToken);
                });
            });

            return services;
        }
    }
}
