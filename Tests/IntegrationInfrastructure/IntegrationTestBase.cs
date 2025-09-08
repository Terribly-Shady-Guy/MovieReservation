namespace Tests.IntegrationInfrastructure
{
    [Collection<WebApplicationFactoryCollectionFixture>]
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
