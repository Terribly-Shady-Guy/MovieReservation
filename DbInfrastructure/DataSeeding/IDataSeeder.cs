using Microsoft.EntityFrameworkCore;

namespace DbInfrastructure.DataSeeding
{
    /// <summary>
    /// A contract for creating a data seeding module. Implement this interface and register the implementation as a scoped service to allow visibility to the seeding provider. 
    /// Both sync and async must be implemented to support flexible execution.
    /// </summary>
    public interface IDataSeeder
    {
        /// <summary>
        /// Seeds data synchronously.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> instance used to apply seeding operations.</param>
        void Add(DbContext context);

        /// <summary>
        /// Seeds data asynchronously.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> instance used to apply seeding operations.</param>
        /// <param name="cancellationToken">The token used to observe cancellation requests for asynchronous operations.</param>
        /// <returns>A <see cref="Task" /> that is resolved when the asynchronous execution finishes.</returns>
        Task AddAsync(DbContext context, CancellationToken cancellationToken);
    }
}
