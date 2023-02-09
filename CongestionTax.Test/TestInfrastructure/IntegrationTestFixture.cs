
namespace CongestionTax.Test.TestInfrastructure
{
    public abstract class IntegrationTestFixture : IAsyncLifetime
    {
        public readonly TestWebApplicationFactory<Program> Factory;

        private readonly SemaphoreSlim _stopSemaphore;

        protected IntegrationTestFixture()
        {
            _stopSemaphore = new SemaphoreSlim(0);
            StopAfter = 1;
            Timeout = TimeSpan.FromSeconds(10);
            Factory = new TestWebApplicationFactory<Program>();
            Factory.CreateClient();
        }

        public int StopAfter { get; set; }

        public TimeSpan Timeout { get; set; }

        // For test controller.
        public HttpClient CreateApiClient()
        {
            var client = Factory.CreateClient();

            return client;
        }

        protected void SignalStop()
        {
            lock (this)
            {
                StopAfter--;

                if (StopAfter <= 0)
                    _stopSemaphore.Release();
            }
        }

        public async Task InitializeAsync()
        {
            // Setup
            await ArrangeAsync();

            // Act
            await ActAsync();
        }

        protected abstract Task ArrangeAsync();

        protected virtual Task ActAsync()
        {
            SignalStop();
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            Factory.Dispose();

            return Task.CompletedTask;
        }
    }
}
