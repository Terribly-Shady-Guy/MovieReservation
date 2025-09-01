using DbInfrastructure;
using DbInfrastructure.DataSeeding;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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

            IExecutionStrategy executionStrategy = context.Database.CreateExecutionStrategy();
            await executionStrategy.ExecuteAsync(async () =>
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.MigrateAsync();
            });
            
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                string testAppSettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.Testing.json");
                config.AddJsonFile(testAppSettingsPath, false, false);
            });

            builder.ConfigureServices((context, services) =>
            {
                services.RemoveAll<IDataSeeder>();
                services.RemoveAll<DbContextOptions<MovieReservationDbContext>>();
                services.AddDbInfrastructure(context.Configuration.GetConnectionString("testing"));
            });

            builder.ConfigureTestServices(services =>
            {
                services.AddTransient<IStartupFilter, TransactionIsolationMiddlewareInjector>();
            });

            builder.UseEnvironment("Development");
        }

        public override async ValueTask DisposeAsync()
        {
            using IServiceScope scope = Services.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<MovieReservationDbContext>();

            IExecutionStrategy executionStrategy = context.Database.CreateExecutionStrategy();
            await executionStrategy.ExecuteAsync(async () =>
            {
                await context.Database.EnsureDeletedAsync();
            });

            await base.DisposeAsync();
        }
    }

    [CollectionDefinition]
    public class WebApplicationFactoryCollectionFixture : ICollectionFixture<MovieReservationWebApplicationFactory>;
}
