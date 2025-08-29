using Microsoft.EntityFrameworkCore;

namespace DbInfrastructure.DataSeeding
{
    internal interface IDataSeeder
    {
        public void Add(DbContext context);
        public Task AddAsync(DbContext context, CancellationToken cancellationToken);
    }
}
