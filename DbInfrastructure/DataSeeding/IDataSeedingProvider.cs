using Microsoft.EntityFrameworkCore;

namespace DbInfrastructure.DataSeeding
{
    /// <summary>
    /// Orchestrates the execution of provided <see cref="IDataSeeder"/> implementations, enabling modular data seeding.
    /// </summary>
    public interface IDataSeedingProvider
    {
        /// <summary>
        /// Executes the registered <see cref="IDataSeeder"/> types synchronously.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> instance used to apply seeding operations.</param>
        void Seed(DbContext context);
        /// <summary>
        /// Executes the registered <see cref="IDataSeeder"/> types asynchronously.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> instance used to apply seeding operations.</param>
        /// <param name="cancellationToken">Token used to observe cancellation requests for asynchronous operations.</param>
        /// <returns>A <see cref="Task" /> that resolves when all seeders have finished execution.</returns>
        Task SeedAsync(DbContext context, CancellationToken cancellationToken);
    }
}