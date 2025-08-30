using DbInfrastructure.DataSeeding;


namespace DbInfrastructure
{
    internal class DataSeedingProvider
    {
        public DataSeedingProvider(List<IDataSeeder> seeders)
        {
            DataSeeders = seeders;
        }

        public IReadOnlyList<IDataSeeder> DataSeeders { get; }
    }
}
