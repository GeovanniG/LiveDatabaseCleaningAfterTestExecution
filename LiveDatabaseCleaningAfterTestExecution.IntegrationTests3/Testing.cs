using Microsoft.Extensions.DependencyInjection;

namespace LiveDatabaseCleaningAfterTestExecution.IntegrationTests;

[TestClass]
public class Testing
{
    private static Dictionary<ConnectionStringEnum, ITestDatabase> _databases = new();
    private static Dictionary<ConnectionStringEnum, ITestDatabaseTransactions> _databaseTransactions = new();
    private static CustomWebApplicationFactory _webFactory = null!;
    private static IServiceScopeFactory _scopeFactory = null!;
    private static IServiceScope _serviceScope = null!;

    public static IReadOnlyDictionary<ConnectionStringEnum, ITestDatabaseTransactions> DatabaseTransactions => _databaseTransactions;
    public static IReadOnlyDictionary<ConnectionStringEnum, ITestDatabase> Databases => _databases;

    [ClassInitialize(InheritanceBehavior.BeforeEachDerivedClass)]
    public static void RunBeforeAnyTests(TestContext context)
    {
        _webFactory = new CustomWebApplicationFactory();
        _scopeFactory = _webFactory.Services.GetRequiredService<IServiceScopeFactory>();
        _serviceScope = _scopeFactory.CreateScope();

        foreach (var connectionString in Enum.GetValues(typeof(ConnectionStringEnum)).Cast<ConnectionStringEnum>())
        {
            var database = TestDatabaseFactory.GetInstanceByConnectionString(
                connectionString, _serviceScope.ServiceProvider);
            _databases.Add(connectionString, database);
            
            var databaseTransaction = TestDatabaseTransactionFactory.GetInstanceByConnectionString(
                connectionString, _serviceScope.ServiceProvider);
            _databaseTransactions.Add(connectionString, databaseTransaction);

            database.Initialize();
        }
    }

    public static void StartState()
    {
        try
        {
            foreach (var database in _databases.Values)
            {
                database.StartTransaction();
            }
        }
        catch (Exception) { }
    }

    public static void ResetState()
    {
        try
        {
            foreach (var database in _databases.Values)
            {
                database.Reset();
            }
        }
        catch (Exception) { }
    }

    [ClassCleanup(InheritanceBehavior.BeforeEachDerivedClass)]
    public static async Task RunAfterAnyTests()
    {
        foreach (var database in _databases.Values)
        {
            database.Dispose();
        }
        _serviceScope.Dispose();
        await _webFactory.DisposeAsync();
    }
}
