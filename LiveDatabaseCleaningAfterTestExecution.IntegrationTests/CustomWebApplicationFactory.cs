using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace LiveDatabaseCleaningAfterTestExecution.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            foreach (var connection in ConnectionStrings.Connections())
            {
                services.AddScoped<ITestDatabase>(
                    x => new SqlServerTestDatabase(connection));
            }

            services.AddScoped<ITestDatabaseFactory, TestDatabaseFactory>();
        });
    }
}
