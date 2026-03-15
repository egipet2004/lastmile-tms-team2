using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace LastMile.TMS.Api.Tests;

/// <summary>
/// WebApplicationFactory that switches the DB to in-memory by overriding the
/// DefaultConnection string to "InMemory". AddPersistence in Persistence/DependencyInjection.cs
/// detects this value and calls UseInMemoryDatabase instead of UseNpgsql.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:DefaultConnection", "InMemory");

        builder.ConfigureTestServices(services =>
        {
            // Remove Hangfire background server to prevent TaskCanceledException during teardown
            var hangfireServer = services.SingleOrDefault(d =>
                d.ImplementationType?.Name == "BackgroundJobServerHostedService");
            if (hangfireServer != null)
                services.Remove(hangfireServer);
        });
    }
}
