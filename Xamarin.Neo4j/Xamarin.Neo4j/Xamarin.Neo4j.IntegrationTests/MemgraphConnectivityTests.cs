using System.Threading.Tasks;
using Neo4j.Driver;
using NUnit.Framework;
using Xamarin.Neo4j.IntegrationTests.Fixtures;

namespace Xamarin.Neo4j.IntegrationTests
{
    [TestFixture]
    public class MemgraphConnectivityTests
    {
        private static MemgraphFixture _fixture;
        private IDriver _driver;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _fixture = new MemgraphFixture();
            await _fixture.StartAsync();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            if (_fixture != null)
                await _fixture.DisposeAsync();
        }

        [SetUp]
        public void SetUp()
        {
            _driver = GraphDatabase.Driver(_fixture.BoltUri);
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_driver != null)
                await _driver.DisposeAsync();
        }

        [Test]
        public async Task CanEstablishConnection()
        {
            await using var session = _driver.AsyncSession();
            var result = await session.RunAsync("RETURN 1 AS n");
            var record = await result.SingleAsync();
            Assert.AreEqual(1L, record["n"].As<long>());
        }

        [Test]
        public async Task CanExecuteCypherQuery()
        {
            await using var session = _driver.AsyncSession();

            var result = await session.RunAsync("CREATE (n:Test {name: 'hello'}) RETURN n.name AS name");
            var record = await result.SingleAsync();

            Assert.AreEqual("hello", record["name"].As<string>());
        }

        [Test]
        public async Task CanQueryNodesAndRelationships()
        {
            await using var session = _driver.AsyncSession();

            await session.RunAsync(
                "CREATE (a:Person {name: 'Alice'})-[:KNOWS]->(b:Person {name: 'Bob'})");

            var cursor = await session.RunAsync(
                "MATCH (a:Person)-[r:KNOWS]->(b:Person) RETURN a.name AS from, b.name AS to, type(r) AS rel");
            var record = await cursor.SingleAsync();

            Assert.AreEqual("Alice", record["from"].As<string>());
            Assert.AreEqual("Bob", record["to"].As<string>());
            Assert.AreEqual("KNOWS", record["rel"].As<string>());
        }
    }
}
