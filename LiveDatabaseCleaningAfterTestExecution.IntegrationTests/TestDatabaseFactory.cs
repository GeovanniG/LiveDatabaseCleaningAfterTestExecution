using Microsoft.Extensions.DependencyInjection;

namespace LiveDatabaseCleaningAfterTestExecution.IntegrationTests;

public class TestDatabaseFactory : ITestDatabaseFactory
{
    private readonly Dictionary<ConnectionStringEnum, ITestDatabase> _testDatabaseByType;

    public TestDatabaseFactory(IServiceProvider serviceProvider)
    {
        _testDatabaseByType = new()
        {
            { ConnectionStringEnum.TestDatabase, serviceProvider.GetRequiredService<SqlServerTestDatabase>() }
        };
    }

    public ITestDatabase GetInstanceByConnectionString(ConnectionStringEnum connectionString)
    {
        return _testDatabaseByType[connectionString];
    }
}