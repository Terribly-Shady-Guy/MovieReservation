using DbInfrastructure.DataSeeding;
using Microsoft.EntityFrameworkCore;


namespace DbInfrastructure
{
    internal class DataSeedingProvider
    {
        private readonly IReadOnlyList<IDataSeeder> _dataSeeders;

        public DataSeedingProvider(List<IDataSeeder> seeders)
        {
            _dataSeeders = seeders;
        }

        public void Seed(DbContext context)
        {
            foreach (IDataSeeder seeder in _dataSeeders)
            {
                seeder.Add(context);
            }
        }

        public async Task SeedAsync(DbContext context, CancellationToken cancellationToken)
        {
            foreach (IDataSeeder seeder in _dataSeeders)
            {
                await seeder.AddAsync(context, cancellationToken);
            }
        }
    }
}
