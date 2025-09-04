namespace Tests.IntegrationInfrastructure
{
    [Collection<WebApplicationFactoryCollectionFixture>]
    public abstract class IntegrationTestBase : IAsyncDisposable
    {
        protected readonly MovieReservationWebApplicationFactory _factory;

        public IntegrationTestBase(MovieReservationWebApplicationFactory factory)
        {
            _factory = factory;
        }

        public virtual async ValueTask DisposeAsync()
        {
            await _factory.ResetDb(TestContext.Current.CancellationToken);
        }
    }
}
