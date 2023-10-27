using Microsoft.Extensions.DependencyInjection;

namespace LiveDatabaseCleaningAfterTestExecution.IntegrationTests;

public static class TestDatabaseFactory
{
    public static ITestDatabase GetInstanceByConnectionString(
        ConnectionStringEnum connectionString,
        IServiceProvider serviceProvider)
    {
        return connectionString switch
        {
            ConnectionStringEnum.TestDatabase => serviceProvider.GetRequiredService<SqlServerTestDatabase>(),
            _ => throw new InvalidOperationException()
        };
    }
}