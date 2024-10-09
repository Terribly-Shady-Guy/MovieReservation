using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MovieReservation.Database
{
    public static class DatabaseInfrastructureConfiguationExtensions
    {
        public static void AddDbInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<MovieReservationDbContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("default"), so =>
                {
                    so.MigrationsAssembly("MovieReservation");
                });
            });
        }
    }
}
