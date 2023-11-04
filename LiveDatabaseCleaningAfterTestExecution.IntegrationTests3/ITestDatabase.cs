using System.Data;

namespace LiveDatabaseCleaningAfterTestExecution.IntegrationTests;

public interface ITestDatabase
{
    void Initialize();

    IDbConnection GetConnection();
    
    IDbTransaction GetTransaction();

    void StartTransaction();

    void Reset();

    void Dispose();
}
