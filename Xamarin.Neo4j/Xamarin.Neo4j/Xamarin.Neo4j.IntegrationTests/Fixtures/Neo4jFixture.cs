using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Neo4j.Driver;

namespace Xamarin.Neo4j.IntegrationTests.Fixtures
{
    public class Neo4jFixture : IAsyncDisposable
    {
        private IContainer _container;

        public string BoltUri { get; private set; }
        public string Username => "neo4j";
        public string Password => "testpassword";

        public async Task StartAsync()
        {
            _container = new ContainerBuilder()
                .WithImage("neo4j:latest")
                .WithPortBinding(7474, true)
                .WithPortBinding(7687, true)
                .WithEnvironment("NEO4J_AUTH", $"{Username}/{Password}")
                .WithWaitStrategy(Wait.ForUnixContainer()
                    .UntilPortIsAvailable(7687))
                .Build();

            await _container.StartAsync();

            var boltPort = _container.GetMappedPublicPort(7687);
            BoltUri = $"bolt://localhost:{boltPort}";

            await WaitForReady();
        }

        private async Task WaitForReady()
        {
            var driver = GraphDatabase.Driver(BoltUri, AuthTokens.Basic(Username, Password));

            for (var i = 0; i < 30; i++)
            {
                try
                {
                    await driver.VerifyConnectivityAsync();
                    await driver.DisposeAsync();
                    return;
                }
                catch
                {
                    await Task.Delay(1000);
                }
            }

            await driver.DisposeAsync();
            throw new TimeoutException("Neo4j container did not become ready in time.");
        }

        public async ValueTask DisposeAsync()
        {
            if (_container != null)
                await _container.DisposeAsync();
        }
    }
}
