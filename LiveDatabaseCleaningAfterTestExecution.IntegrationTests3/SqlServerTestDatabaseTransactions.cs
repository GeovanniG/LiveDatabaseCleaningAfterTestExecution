using Dapper;
using System.Data;

namespace LiveDatabaseCleaningAfterTestExecution.IntegrationTests;

public class SqlServerTestDatabaseTransactions : ITestDatabaseTransactions
{
    private readonly ITestDatabase _testDatabase;

    public SqlServerTestDatabaseTransactions(ITestDatabase testDatabase)
    {
        _testDatabase = testDatabase;
    }

    public async Task<int> ExecuteWithTransactionAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        try
        {
            var affectedRows = await _testDatabase.GetConnection().ExecuteAsync(sql, param, _testDatabase.GetTransaction(), commandTimeout, commandType);
            return affectedRows;
        }
        catch
        {
            _testDatabase.GetTransaction().Rollback();
            throw;
        }
    }

    public async Task<T?> QueryFirstOrDefaultWithTransactionAsync<T>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        try
        {
            var result = await _testDatabase.GetConnection().QueryFirstOrDefaultAsync<T>(sql, param, _testDatabase.GetTransaction(), commandTimeout, commandType);
            return result;
        }
        catch
        {
            _testDatabase.GetTransaction().Rollback();
            throw;
        }
    }
}
