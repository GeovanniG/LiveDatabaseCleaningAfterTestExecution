using System.Data;

namespace LiveDatabaseCleaningAfterTestExecution.IntegrationTests;

public interface ITestDatabase
{
    Task InitialiseAsync();

    IDbConnection GetConnection();

    Task ResetAsync();

    Task DisposeAsync();
}
