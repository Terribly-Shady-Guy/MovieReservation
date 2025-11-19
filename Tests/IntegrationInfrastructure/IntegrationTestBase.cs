namespace Tests.IntegrationInfrastructure
{
    /// <summary>
    /// Provides the base infrastructure for integration tests.
    /// </summary>
    [Collection<MovieReservationIntegrationTestsFixture>]
    public abstract class IntegrationTestBase : IAsyncDisposable
    {
        public IntegrationTestBase(MovieReservationWebApplicationFactory factory)
        {
            Factory = factory;
        }

        protected MovieReservationWebApplicationFactory Factory { get; }

        public virtual async ValueTask DisposeAsync()
        {
            await Factory.ResetDb(TestContext.Current.CancellationToken);
        }
    }
}
