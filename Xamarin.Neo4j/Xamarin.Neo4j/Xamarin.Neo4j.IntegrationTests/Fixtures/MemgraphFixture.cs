using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Neo4j.Driver;

namespace Xamarin.Neo4j.IntegrationTests.Fixtures
{
    public class MemgraphFixture : IAsyncDisposable
    {
        private IContainer _container;

        public string BoltUri { get; private set; }

        public async Task StartAsync()
        {
            _container = new ContainerBuilder()
                .WithImage("memgraph/memgraph:latest")
                .WithPortBinding(7687, true)
                .WithCommand("--bolt-server-name-for-init=Neo4j/5.0.0", "--also-log-to-stderr")
                .WithWaitStrategy(Wait.ForUnixContainer()
                    .UntilPortIsAvailable(7687))
                .Build();

            await _container.StartAsync();

            var boltPort = _container.GetMappedPublicPort(7687);
            BoltUri = $"bolt://localhost:{boltPort}";

            Console.Error.WriteLine($"[MemgraphFixture] Container started, BoltUri={BoltUri}");

            await WaitForReady();

            Console.Error.WriteLine("[MemgraphFixture] Ready!");
        }

        private async Task WaitForReady()
        {
            for (var i = 0; i < 30; i++)
            {
                IDriver driver = null;
                try
                {
                    driver = GraphDatabase.Driver(BoltUri);
                    await using var session = driver.AsyncSession();
                    var cursor = await session.RunAsync("RETURN 1 AS n");
                    await cursor.ConsumeAsync();
                    return;
                }
                catch (Exception ex)
                {
                    var inner = ex.InnerException != null ? $" -> {ex.InnerException.GetType().Name}: {ex.InnerException.Message}" : "";
                    Console.Error.WriteLine($"[Memgraph] Attempt {i}: {ex.GetType().Name}: {ex.Message}{inner}");
                    await Task.Delay(2000);
                }
                finally
                {
                    if (driver != null) await driver.DisposeAsync();
                }
            }

            throw new TimeoutException("Memgraph container did not become ready in time.");
        }

        public async ValueTask DisposeAsync()
        {
            if (_container != null)
                await _container.DisposeAsync();
        }
    }
}
