namespace LiveDatabaseCleaningAfterTestExecution.IntegrationTests;

public interface ITestDatabaseFactory
{
    ITestDatabase GetInstanceByConnectionString(ConnectionStringEnum connectionString);
}
