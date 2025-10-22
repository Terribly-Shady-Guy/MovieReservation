using Microsoft.EntityFrameworkCore;


namespace DbInfrastructure.DataSeeding
{
    public sealed class DataSeedingExecutor : IDataSeedingExecutor
    {
        private readonly IReadOnlyList<IDataSeeder> _dataSeeders;

        public DataSeedingExecutor(IReadOnlyList<IDataSeeder> seeders)
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

        /// <inheritdoc/>
        /// <exception cref="OperationCanceledException"/>
        public async Task SeedAsync(DbContext context, CancellationToken cancellationToken)
        {
            foreach (IDataSeeder seeder in _dataSeeders)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await seeder.AddAsync(context, cancellationToken);
            }
        }
    }
}
