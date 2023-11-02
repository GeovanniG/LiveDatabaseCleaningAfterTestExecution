using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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

            //ScriptDatabase(
            //    _connectionString, "AdventureWorks2019",
            //    _sqlServerContainer.GetConnectionString(), "AdventureWorks2019");

            //await ScriptDatabaseAsync(".", "AdventureWorks2019", "devuser", "Test1234!");
            await ScriptDatabaseAsync(_connectionString, "AdventureWorks2019");

            _connection = new SqlConnection(_sqlServerContainer.GetConnectionString());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private void ScriptDatabase(
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

    private async Task ScriptDatabaseAsync(string sourceConnectionString, string sourceDatabaseName,
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


    private async Task ScriptDatabaseAsync(string sourceConnectionString, string sourceDatabase)
    {
        using (var sourceConnection = new SqlConnection(sourceConnectionString))
        {
            await sourceConnection.OpenAsync();
            var server = new Server(new ServerConnection(sourceConnection));

            // Connect to the database
            Database database = server.Databases[sourceDatabase];

            // Define the SQL script
            string sqlScript = @$"
            USE {sourceDatabase};

            -- Retrieve information about tables
            SELECT * FROM information_schema.tables;

            -- Retrieve information about stored procedures
            SELECT * FROM information_schema.routines WHERE routine_type = 'PROCEDURE';

            -- Retrieve information about schemas
            SELECT * FROM information_schema.schemata;

            -- Retrieve information about user-defined types/functions
            SELECT * FROM sys.types WHERE is_user_defined = 1;

            -- Retrieve information about views
            SELECT * FROM information_schema.views;
            ";

            // Execute the SQL script
            var result = database.ExecuteWithResults(sqlScript);
        }
    }
    private async Task ScriptDatabaseAsync(string sourceServer, string sourceDatabase, string userName, string password)
    {
        string sourceConnectionString = $"Server={sourceServer};Database={sourceDatabase};User Id={userName};Password={password};TrustServerCertificate=True;";

        // Specify the path for the generated SQL script
        string scriptFilePath = "SchemaAndObjects.sql";

        // Generate SQL script for schema and objects using sqlcmd
        string generateScriptCommand = @$"sqlcmd -S {sourceServer} -d {sourceDatabase} -U {userName} -P {password} -Q ""SET NOCOUNT ON; :setvar SQLCMDMODE DISABLED; :OUT {scriptFilePath}""";
        var script = @$"sqlcmd -S {sourceServer} -d {sourceDatabase} -U {userName} -P {password} -Q ""USE SourceDatabase; SELECT definition FROM sys.sql_modules WHERE type_desc = 'VIEW'"" -o {scriptFilePath}";

        await RunCommandAsync(generateScriptCommand);

        var scriptFileName = Path.GetFileName(scriptFilePath);
        await _sqlServerContainer.CopyAsync(scriptFilePath, $"/var/opt/mssql/script/{scriptFileName}");

        await _sqlServerContainer.ExecScriptAsync("sqlcmd -S localhost -U sa -P yourStrong(!)Password -i /var/opt/mssql/script/" + scriptFileName);
    }

    private async Task RunCommandAsync(string command)
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using (Process process = new Process { StartInfo = psi })
        {
            process.Start();

            await process.StandardInput.WriteLineAsync(command);
            await process.StandardInput.FlushAsync();
            process.StandardInput.Close();

            await process.WaitForExitAsync();
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