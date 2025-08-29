namespace DbInfrastructure.DataSeeding
{
    internal interface IDataSeedingHelper
    {
        public void Add();
        public Task AddAsync(CancellationToken cancellationToken);
    }
}
