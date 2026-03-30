using System.Threading.Tasks;
using Neo4j.Driver;
using NUnit.Framework;
using Xamarin.Neo4j.IntegrationTests.Fixtures;

namespace Xamarin.Neo4j.IntegrationTests
{
    [TestFixture]
    public class Neo4jConnectivityTests
    {
        private static Neo4jFixture _fixture;
        private IDriver _driver;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _fixture = new Neo4jFixture();
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
            _driver = GraphDatabase.Driver(
                _fixture.BoltUri,
                AuthTokens.Basic(_fixture.Username, _fixture.Password));
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
            await _driver.VerifyConnectivityAsync();
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

        [Test]
        public async Task CanExpandNode()
        {
            await using var session = _driver.AsyncSession();

            var setupResult = await session.RunAsync(
                "CREATE (c:Hub {name: 'Center'})" +
                "CREATE (c)-[:LINK]->(a:Leaf {name: 'A'})" +
                "CREATE (c)-[:LINK]->(b:Leaf {name: 'B'})" +
                "CREATE (c)-[:LINK]->(d:Leaf {name: 'C'})" +
                "RETURN id(c) AS centerId");
            var setupRecord = await setupResult.SingleAsync();
            var centerId = setupRecord["centerId"].As<long>();

            var cursor = await session.RunAsync(
                "MATCH (n)-[r]-(m) WHERE id(n) = $nodeId RETURN n, r, m",
                new { nodeId = centerId });

            var records = await cursor.ToListAsync();

            Assert.AreEqual(3, records.Count);

            foreach (var record in records)
            {
                Assert.IsInstanceOf<INode>(record["n"]);
                Assert.IsInstanceOf<IRelationship>(record["r"]);
                Assert.IsInstanceOf<INode>(record["m"]);
            }
        }

        [Test]
        public void CanParseConnectionString()
        {
            var connectionString = new Models.Neo4jConnectionString
            {
                Scheme = "bolt://",
                Host = "localhost:7687",
                Username = _fixture.Username,
                Password = _fixture.Password,
                Database = "neo4j"
            };

            var result = connectionString.ParseHost();

            Assert.AreEqual("bolt://localhost:7687", result.Item1);
            Assert.IsFalse(result.Item2);
            Assert.IsFalse(result.Item3);
        }
    }
}
