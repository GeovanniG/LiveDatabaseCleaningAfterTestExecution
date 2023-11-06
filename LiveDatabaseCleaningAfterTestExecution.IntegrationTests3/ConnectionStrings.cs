using System.Runtime.CompilerServices;

namespace LiveDatabaseCleaningAfterTestExecution.IntegrationTests;

public class ConnectionStrings
{
    public static string TestDatabase => "Data Source=.;Initial Catalog=AdventureWorks2022;Integrated Security=True;TrustServerCertificate=True;";

    public static IEnumerable<string> Connections()
    {
        yield return TestDatabase;
    }

}
