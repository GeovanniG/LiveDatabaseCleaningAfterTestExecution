using System.Runtime.CompilerServices;

namespace LiveDatabaseCleaningAfterTestExecution.IntegrationTests;

public class ConnectionStrings
{
    public static string TestDatabase => "Data Source=.;Initial Catalog=AdventureWorks2019;User Id=devuser;Password=Test1234!;TrustServerCertificate=True;";

    public static IEnumerable<string> Connections()
    {
        yield return TestDatabase;
    }

}
