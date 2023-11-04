using Dapper;
using LiveDatabaseCleaningAfterTestExecution.Entity;

namespace LiveDatabaseCleaningAfterTestExecution.IntegrationTests;

[TestClass]
public class UnitTest1 : BaseTestFixture
{

    [TestMethod]
    public async Task Method1()
    {
        // Arrange
        var updatedEmailAddress = "khanh1@adventure-works.com";
        var businessEntityId = 1;
        var database = Databases.Where(x => x.Key == ConnectionStringEnum.TestDatabase)
            .Select(x => x.Value).First();
        await database.ExecuteWithTransactionAsync(
            "UPDATE Person.EmailAddress SET EmailAddress = @EmailAddress WHERE BusinessEntityID = @BusinessEntityId",
            new { EmailAddress = updatedEmailAddress, BusinessEntityId = businessEntityId });

        // Act
        var email = await database.QueryFirstOrDefaultWithTransactionAsync<Email>(
            "SELECT BusinessEntityID, EmailAddressID, EmailAddress, ModifiedDate FROM Person.EmailAddress WHERE BusinessEntityID = @BusinessEntityId",
            new { BusinessEntityId = businessEntityId });

        // Assert
        email.Should().NotBeNull();
        email!.EmailAddress.Should().Be(updatedEmailAddress);
    }
}