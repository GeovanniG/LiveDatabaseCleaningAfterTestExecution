namespace LiveDatabaseCleaningAfterTestExecution.IntegrationTests;


[TestClass]
public class BaseTestFixture : Testing
{
    [TestCleanup]
    public async Task TestSetup()
    {
        await ResetState();
    }
}
