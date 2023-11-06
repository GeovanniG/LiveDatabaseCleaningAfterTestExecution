using Microsoft.Extensions.DependencyInjection;

namespace LiveDatabaseCleaningAfterTestExecution.IntegrationTests;

public class TestDatabaseTransactionFactory
{
    public static ITestDatabaseTransactions GetInstanceByConnectionString(
        ConnectionStringEnum connectionString,
        IServiceProvider serviceProvider)
    {
        return connectionString switch
        {
            ConnectionStringEnum.TestDatabase => serviceProvider.GetRequiredService<SqlServerTestDatabaseTransactions>(),
            _ => throw new InvalidOperationException()
        };
    }
}
