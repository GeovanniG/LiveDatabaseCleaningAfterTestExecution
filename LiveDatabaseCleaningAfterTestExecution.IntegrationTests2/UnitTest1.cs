using Dapper;

namespace LiveDatabaseCleaningAfterTestExecution.IntegrationTests;

[TestClass]
public class UnitTest1 : BaseTestFixture
{

    [TestMethod]

    public async Task Method1()
    {
        var sql = "UPDATE Person.EmailAddress SET EmailAddress = 'khanh0@adventure-works.com' WHERE BusinessEntityID = 1";
        var database = Databases.Where(x => x.Key == ConnectionStringEnum.TestDatabase)
            .Select(x => x.Value).FirstOrDefault();
        var data0 = await database.GetConnection().QueryAsync("SELECT table_name = t.name FROM sys.tables t INNER JOIN sys.schemas s ON t.schema_id = s.schema_id;");
        await database.GetConnection().ExecuteAsync(sql);
        var data = await database.GetConnection().QueryAsync("SELECT * FROM Person.EmailAddress");
    }
}