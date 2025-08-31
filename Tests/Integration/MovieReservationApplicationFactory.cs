using DbInfrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Tests.Integration
{
    internal class MovieReservationWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.RemoveAll<DbContextOptions<MovieReservationDbContext>>();
                services.AddDbInfrastructure(context.Configuration.GetConnectionString("testing"));
            });

            builder.UseEnvironment("Development");
        }
    }

    [CollectionDefinition]
    public class WebApplicationFactoryCollectionFixture : ICollectionFixture<MovieReservationWebApplicationFactory>;
}
