using Dapper;
using System.Data;

namespace LiveDatabaseCleaningAfterTestExecution.IntegrationTestsTransactions;

public abstract class BaseTestDatabase
{
    public async Task<int> ExecuteWithTransactionAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        try
        {
            var affectedRows = await GetConnection().ExecuteAsync(sql, param, GetTransaction(), commandTimeout, commandType);
            return affectedRows;
        }
        catch
        {
            GetTransaction().Rollback();
            throw;
        }
    }

    public async Task<T?> QueryFirstOrDefaultWithTransactionAsync<T>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        try
        {
            var result = await GetConnection().QueryFirstOrDefaultAsync<T>(sql, param, GetTransaction(), commandTimeout, commandType);
            return result;
        }
        catch
        {
            GetTransaction().Rollback();
            throw;
        }
    }

    public abstract IDbConnection GetConnection();

    public abstract IDbTransaction GetTransaction();
}
