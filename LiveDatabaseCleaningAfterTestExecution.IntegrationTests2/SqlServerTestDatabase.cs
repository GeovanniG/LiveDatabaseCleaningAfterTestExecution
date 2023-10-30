using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Collections.Specialized;
using System.Data;
using Testcontainers.MsSql;

namespace LiveDatabaseCleaningAfterTestExecution.IntegrationTests;

public class SqlServerTestDatabase : ITestDatabase
{
    private readonly string _connectionString;
    private MsSqlContainer _sqlServerContainer = null!;
    private IDbConnection _connection = null!;

    public SqlServerTestDatabase(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task InitialiseAsync()
    {
        _sqlServerContainer = new MsSqlBuilder().WithPortBinding(1433, 1433).Build();

        try
        {
            await _sqlServerContainer.StartAsync();

            //await ScriptDatabaseAsync(
            //    _connectionString, "AdventureWorks2019",
            //    _sqlServerContainer.GetConnectionString());

            ScriptDatabaseAsync(
                _connectionString, "AdventureWorks2019",
                _sqlServerContainer.GetConnectionString(), "AdventureWorks2019");

            _connection = new SqlConnection(_sqlServerContainer.GetConnectionString());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void ScriptDatabaseAsync(
        string sourceConnectionString, string sourceDatabaseName,
        string destinationConnectionString, string destinationDatabaseName)
    {
        Server sourceServer = new Server(new ServerConnection(new SqlConnection(sourceConnectionString)));
        Database sourceDatabase = sourceServer.Databases[sourceDatabaseName];

        Server destinationServer = new Server(new ServerConnection(new SqlConnection(destinationConnectionString)));
        //Database destinationDatabase = new Database(destinationServer, destinationDatabaseName);
        //destinationDatabase.Create();

        // Transfer schema and objects
        Transfer transfer = new Transfer(sourceDatabase);
        transfer.CopyAllObjects = true;
        transfer.Options.WithDependencies = true;
        transfer.DestinationServer = destinationServer.Name;
        transfer.DestinationDatabase = "master";
        transfer.CopySchema = true;
        transfer.Options.ScriptDrops = false;
        transfer.Options.ScriptData = false;

        transfer.TransferData();
    }

    static async Task ScriptDatabaseAsync(string sourceConnectionString, string sourceDatabaseName,
        string destinationConnectionString)
    {
        using (var sourceConnection = new SqlConnection(sourceConnectionString))
        {
            await sourceConnection.OpenAsync();
            var server = new Server(new ServerConnection(sourceConnection));
            var db = server.Databases[sourceDatabaseName];

            var script = new Scripter(server);
            var scriptOpt = new ScriptingOptions();
            scriptOpt.IncludeHeaders = true;
            scriptOpt.SchemaQualify = true;

            using (var destinationConnection = new SqlConnection(destinationConnectionString))
            {
                var sql = new SqlCommand();
                sql.Connection = destinationConnection;
                await destinationConnection.OpenAsync();

                foreach (Schema schema in db.Schemas)
                {
                    if (!schema.IsSystemObject)
                    {
                        StringCollection sc = schema.Script(scriptOpt);
                        foreach (string s in sc)
                        {
                            sql.CommandText = s;
                            await sql.ExecuteNonQueryAsync();
                        }
                    }
                }

                foreach (XmlSchemaCollection xmlSchema in db.XmlSchemaCollections)
                {
                    StringCollection sc = xmlSchema.Script(scriptOpt);
                    foreach (string s in sc)
                    {
                        sql.CommandText = s;
                        await sql.ExecuteNonQueryAsync();
                    }
                }

                foreach (UserDefinedDataType udt in db.UserDefinedDataTypes)
                {
                    StringCollection sc = udt.Script(scriptOpt);
                    foreach (string s in sc)
                    {
                        sql.CommandText = s;
                        await sql.ExecuteNonQueryAsync();
                    }
                }

                foreach (UserDefinedDataType udt in db.UserDefinedTypes)
                {
                    StringCollection sc = udt.Script(scriptOpt);
                    foreach (string s in sc)
                    {
                        sql.CommandText = s;
                        await sql.ExecuteNonQueryAsync();
                    }
                }

                foreach (UserDefinedFunction udf in db.UserDefinedFunctions)
                {
                    if (!udf.IsSystemObject)
                    {
                        StringCollection sc = udf.Script(scriptOpt);
                        foreach (string s in sc)
                        {
                            sql.CommandText = s;
                            await sql.ExecuteNonQueryAsync();
                        }
                    }
                }

                foreach (Table table in db.Tables)
                {
                    if (!table.IsSystemObject)
                    {
                        StringCollection sc = table.Script(scriptOpt);
                        foreach (string s in sc)
                        {
                            sql.CommandText = s;
                            await sql.ExecuteNonQueryAsync();
                        }
                    }
                }

                foreach (View view in db.Views)
                {
                    if (!view.IsSystemObject)
                    {
                        StringCollection sc = view.Script(scriptOpt);
                        foreach (string s in sc)
                        {
                            sql.CommandText = s;
                            await sql.ExecuteNonQueryAsync();
                        }
                    }
                }

                foreach (StoredProcedure sp in db.StoredProcedures)
                {
                    if (!sp.IsSystemObject)
                    {
                        StringCollection sc = sp.Script(scriptOpt);
                        foreach (string s in sc)
                        {
                            try
                            {
                                sql.CommandText = s;
                                await sql.ExecuteNonQueryAsync();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                        }
                    }
                }
            }
        }
    }

    public IDbConnection GetConnection()
    {
        return _connection;
    }

    public async Task ResetAsync()
    {
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _connection?.Close();
        await _sqlServerContainer.StopAsync();
    }
}