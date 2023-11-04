using LiveDatabaseCleaningAfterTestExecution.IntegrationTestsTransactions;
using System.Data;
using System.Data.SqlClient;

namespace LiveDatabaseCleaningAfterTestExecution.IntegrationTests;

public class SqlServerTestDatabase : BaseTestDatabase, ITestDatabase
{
    private readonly string _connectionString = null!;
    private IDbConnection _connection = null!;
    private IDbTransaction _transaction = null!;

    public SqlServerTestDatabase(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Initialize()
    {
        _connection = new SqlConnection(_connectionString);
    }

    public override IDbConnection GetConnection()
    {
        return _connection ?? throw new ArgumentNullException(nameof(_connection));
    }

    public override IDbTransaction GetTransaction()
    {
        return _transaction ?? throw new ArgumentNullException(nameof(_transaction));
    }

    public void StartTransaction()
    {
        _connection.Open();
        _transaction = _connection.BeginTransaction();
    }

    public void Reset()
    {
        _transaction.Rollback();
    }

    public void Dispose()
    {
        _connection.Close();
    }
}