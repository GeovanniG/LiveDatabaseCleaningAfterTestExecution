using Microsoft.Data.SqlClient;
using Respawn;
using System.Data;

namespace LiveDatabaseCleaningAfterTestExecution.IntegrationTests;

public class SqlServerTestDatabase : ITestDatabase
{
    private readonly string _connectionString = null!;
    private IDbConnection _connection = null!;
    private Respawner _respawner = null!;

    public SqlServerTestDatabase(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task InitialiseAsync()
    {
        _connection = new SqlConnection(_connectionString);

        _respawner = await Respawner.CreateAsync(_connectionString);
    }

    public IDbConnection GetConnection()
    {
        return _connection;
    }

    public async Task ResetAsync()
    {
        await _respawner.ResetAsync(_connectionString);
    }

    public async Task DisposeAsync()
    {
        _connection.Close();
        await Task.CompletedTask;
    }
}