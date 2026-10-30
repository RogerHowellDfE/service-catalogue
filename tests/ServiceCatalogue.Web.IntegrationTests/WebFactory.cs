using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ServiceCatalogue.Web.IntegrationTests;

/// <summary>Runs the real Web app in the given environment against a database that refuses connections.</summary>
public sealed class WebFactory(string environment) : WebApplicationFactory<Program>
{
    // Port 1 refuses connections immediately; no password because nothing real is behind it.
    public const string UnreachableDatabase = "Host=localhost;Port=1;Database=fake;Timeout=2";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(environment);
        builder.UseSetting("ConnectionStrings:servicecatalogue", UnreachableDatabase);
    }
}
