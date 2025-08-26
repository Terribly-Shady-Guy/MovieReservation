using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MovieReservation.Startup
{
    public class SqlServerHealthCheck : IHealthCheck
    {
        private readonly string? _connectionString;

        public SqlServerHealthCheck(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("default");
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (_connectionString == null)
            {
                return HealthCheckResult.Unhealthy("The connection string is missing.");
            }
            
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                using SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT 1";

                _ = await command.ExecuteScalarAsync(cancellationToken);

                return HealthCheckResult.Healthy("Db connected and executed query successfully.");
            }
            catch (SqlException ex) when (ex.Number == -2)
            {  
                return HealthCheckResult.Degraded("The connection timed out.", ex);
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("An error has occured when connecting to db.", ex);
            }
        }
    }
}
