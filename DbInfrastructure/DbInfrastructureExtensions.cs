using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace DbInfrastructure
{
    public static class DbInfrastructureExtensions
    {
        /// <summary>
        /// Adds <c>MovieReservationDbContext</c> and configures ef core using the provided connection string.
        /// </summary>
        /// <param name="services">The service collection instance used by the app.</param>
        /// <param name="connectionString">The connection string for SQL Server</param>
        public static void AddDbInfrastructure(this IServiceCollection services, string? connectionString)
        {
            services.AddDbContext<MovieReservationDbContext>(options =>
            {
                options.UseSqlServer(connectionString, config =>
                {
                    config.EnableRetryOnFailure();
                });

                options.UseDataSeeding();
            });
        }
    }
}
