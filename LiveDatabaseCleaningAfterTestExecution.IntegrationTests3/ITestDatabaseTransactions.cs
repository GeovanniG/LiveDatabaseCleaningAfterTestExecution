using System.Data;

namespace LiveDatabaseCleaningAfterTestExecution.IntegrationTests;

public interface ITestDatabaseTransactions
{
    Task<int> ExecuteWithTransactionAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null);
    Task<T?> QueryFirstOrDefaultWithTransactionAsync<T>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null);
}