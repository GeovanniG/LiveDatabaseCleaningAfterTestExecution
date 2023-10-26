using Microsoft.Extensions.DependencyInjection;

namespace LiveDatabaseCleaningAfterTestExecution.IntegrationTests;

[TestClass]
public partial class Testing
{
    private static Dictionary<ConnectionStringEnum, ITestDatabase> _databases = new();
    public static IReadOnlyDictionary<ConnectionStringEnum, ITestDatabase> Databases => _databases;
    
    private static CustomWebApplicationFactory _webFactory = null!;
    private static IServiceScopeFactory _scopeFactory = null!;

    private readonly ITestDatabaseFactory _testDatabaseFactory;

    public Testing(ITestDatabaseFactory testDatabaseFactory)
    {
        _testDatabaseFactory = testDatabaseFactory;
    }

    [ClassInitialize()]
    public async Task RunBeforeAnyTests(TestContext context)
    {
        foreach (var connectionString in Enum.GetValues(typeof(ConnectionStringEnum)).Cast<ConnectionStringEnum>())
        {
            var database = _testDatabaseFactory.GetInstanceByConnectionString(connectionString);
            await database.InitialiseAsync();
            _databases.Add(connectionString, database);
        }

        _webFactory = new CustomWebApplicationFactory();

        _scopeFactory = _webFactory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    public static async Task ResetState()
    {
        try
        {
            foreach (var database in _databases.Values)
            {
                await database.ResetAsync();
            }
        }
        catch (Exception) { }
    }

    [ClassCleanup()]
    public async Task RunAfterAnyTests()
    {
        foreach (var database in _databases.Values)
        {
            await database.DisposeAsync();
        }

        await _webFactory.DisposeAsync();
    }
}
