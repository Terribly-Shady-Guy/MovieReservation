using DbInfrastructure;
using DbInfrastructure.DataSeeding;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Respawn;

namespace Tests.IntegrationInfrastructure
{
    public class MovieReservationWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private Respawner? _respawner;
        private string _connectionString = string.Empty;

        public async ValueTask InitializeAsync()
        {
            using IServiceScope scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MovieReservationDbContext>();

            await context.Database.EnsureDeletedAsync();
            await context.Database.MigrateAsync();

            _connectionString = context.Database.GetConnectionString() ?? throw new InvalidOperationException("The connection string is null.");

            _respawner = await Respawner.CreateAsync(_connectionString, new RespawnerOptions
            {
                WithReseed = true,
                DbAdapter = DbAdapter.SqlServer,
                TablesToIgnore = [
                    "__EFMigrationsHistory",
                    "ReservationStatus",
                    "TheaterTypes"
                    ]
            });
        }

        public async Task ResetDb(CancellationToken token)
        {
            if (_respawner == null) 
            {
                throw new InvalidOperationException("The respawn instance was not initialized.");
            }

            using IServiceScope scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MovieReservationDbContext>();

            await _respawner.ResetAsync(_connectionString);

            var seedingProvider = scope.ServiceProvider.GetRequiredService<IDataSeedingProvider>();
            await seedingProvider.SeedAsync(context, token);
            await context.SaveChangesAsync(token);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((config) =>
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

            builder.UseEnvironment("Testing");
        }

        public override async ValueTask DisposeAsync()
        {
            using (IServiceScope scope = Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<MovieReservationDbContext>();
                await context.Database.EnsureDeletedAsync();
            }

            await base.DisposeAsync();
        }
    }

    [CollectionDefinition]
    public class WebApplicationFactoryCollectionFixture : ICollectionFixture<MovieReservationWebApplicationFactory>;
}
