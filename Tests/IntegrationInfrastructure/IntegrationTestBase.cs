namespace Tests.IntegrationInfrastructure
{
    [Collection<WebApplicationFactoryCollectionFixture>]
    public abstract class IntegrationTestBase : IAsyncDisposable
    {
        protected MovieReservationWebApplicationFactory Factory { get; }

        public IntegrationTestBase(MovieReservationWebApplicationFactory factory)
        {
            Factory = factory;
        }

        public virtual async ValueTask DisposeAsync()
        {
            await Factory.ResetDb(TestContext.Current.CancellationToken);
        }
    }
}
