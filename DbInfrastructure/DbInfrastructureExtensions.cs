using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MovieReservation.Data.DbContexts;

namespace DbInfrastructure
{
    public static class DbInfrastructureExtensions
    {
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
