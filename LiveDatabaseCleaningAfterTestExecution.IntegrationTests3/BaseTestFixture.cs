namespace LiveDatabaseCleaningAfterTestExecution.IntegrationTests;


[TestClass]
public class BaseTestFixture : Testing
{
    [TestInitialize]
    public void TestSetup()
    {
        StartState();
    }

    [TestCleanup]
    public void TestCleanUp()
    {
        ResetState();
    }
}
