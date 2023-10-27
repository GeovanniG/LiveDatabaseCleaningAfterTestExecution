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
        await database.GetConnection().ExecuteAsync(sql);
    }
}