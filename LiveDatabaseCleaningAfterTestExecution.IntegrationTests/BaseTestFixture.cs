namespace LiveDatabaseCleaningAfterTestExecution.IntegrationTests;

using static Testing;

[TestClass]
public class BaseTestFixture
{
    [TestInitialize]
    public async Task TestSetup()
    {
        await ResetState();
    }
}
