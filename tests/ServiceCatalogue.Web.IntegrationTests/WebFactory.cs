using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceCatalogue.Web.IntegrationTests;

/// <summary>Runs the real Web app in the given environment against a database that refuses connections.</summary>
public sealed class WebFactory(string environment, Action<IServiceCollection>? configureServices = null) : WebApplicationFactory<Program>
{
    // Port 1 refuses connections immediately; no password because nothing real is behind it.
    public const string UnreachableDatabase = "Host=localhost;Port=1;Database=fake;Timeout=2";

    public const string ThrowingPath = "/test/throw";

    public const string FakeFailureMessage = "Fake failure raised for Test Teri";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(environment);
        builder.UseSetting("ConnectionStrings:servicecatalogue", UnreachableDatabase);
        builder.ConfigureServices(services =>
        {
            services.AddTransient<IStartupFilter, ThrowingEndpoint>();
            configureServices?.Invoke(services);
        });
    }

    /// <summary>Appends a route after the app's own pipeline that always throws, to exercise its error handling.</summary>
    private sealed class ThrowingEndpoint : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => app =>
        {
            next(app);
            app.Map(ThrowingPath, branch => branch.Run(_ => throw new InvalidOperationException(FakeFailureMessage)));
        };
    }
}
