using DbInfrastructure;
using DbInfrastructure.DataSeeding;
using DbInfrastructure.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Respawn;
using Respawn.Graph;

namespace Tests.IntegrationInfrastructure
{
    public class MovieReservationWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private Respawner? _respawner;

        public async ValueTask InitializeAsync()
        {
            using IServiceScope scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MovieReservationDbContext>();

            await context.Database.EnsureDeletedAsync();
            await context.Database.MigrateAsync();

            var connection = context.Database.GetDbConnection();

            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            var tablesToIgnore = context.Model.GetEntityTypes()
                .Where(et => et.ClrType.BaseType is not null
                                && et.ClrType.BaseType.IsGenericType
                                && et.ClrType.BaseType.GetGenericTypeDefinition() == typeof(EnumLookupBase<>))
                .Select(et => et.GetTableName())
                .OfType<string>();


            _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
            {
                WithReseed = true,
                DbAdapter = DbAdapter.SqlServer,
                TablesToIgnore = [
                    ..tablesToIgnore,
                    "__EFMigrationsHistory"
                    ],
            });
        }

        public async ValueTask ResetDb(CancellationToken token)
        {
            if (_respawner == null) 
            {
                throw new InvalidOperationException("The respawn instance was not initialized.");
            }

            using IServiceScope scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MovieReservationDbContext>();

            var connection = context.Database.GetDbConnection();

            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync(token);
            }

            await _respawner.ResetAsync(connection);

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
