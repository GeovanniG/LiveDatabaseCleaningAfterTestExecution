﻿using System.Runtime.CompilerServices;

namespace LiveDatabaseCleaningAfterTestExecution.IntegrationTests;

public class ConnectionStrings
{
    public static string TestDatabase => "Data Source=.;Initial Catalog=AdventureWorks2019;Integrated Security=True;";

    public static IEnumerable<string> Connections()
    {
        yield return TestDatabase;
    }

}
