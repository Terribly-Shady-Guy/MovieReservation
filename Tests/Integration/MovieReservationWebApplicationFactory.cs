using DbInfrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Tests.Integration
{
    public class MovieReservationWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        public async ValueTask InitializeAsync()
        {
            using IServiceScope scope = Services.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<MovieReservationDbContext>();

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.RemoveAll<DbContextOptions<MovieReservationDbContext>>();
                services.AddDbInfrastructure(context.Configuration.GetConnectionString("testing"));
            });

            builder.UseEnvironment("Development");
        }

        public override async ValueTask DisposeAsync()
        {
            using IServiceScope scope = Services.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<MovieReservationDbContext>();

            await context.Database.EnsureDeletedAsync();

            await base.DisposeAsync();
        }
    }

    [CollectionDefinition]
    public class WebApplicationFactoryCollectionFixture : ICollectionFixture<MovieReservationWebApplicationFactory>;
}
