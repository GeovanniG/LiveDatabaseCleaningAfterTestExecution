﻿using System.Data;

namespace LiveDatabaseCleaningAfterTestExecution.IntegrationTests;

public interface ITestDatabase
{
    void Initialize();

    void StartTransaction();

    public abstract IDbConnection GetConnection();

    public abstract IDbTransaction GetTransaction();

    void Reset();

    void Dispose();

    Task<int> ExecuteWithTransactionAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null);

    Task<T?> QueryFirstOrDefaultWithTransactionAsync<T>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null);
}
