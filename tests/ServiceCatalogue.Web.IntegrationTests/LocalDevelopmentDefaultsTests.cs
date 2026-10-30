using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceCatalogue.Web.IntegrationTests;

/// <summary>Defaults ASP.NET Core only switches on in Development are switched on in LocalDevelopment too (#11).</summary>
public class LocalDevelopmentDefaultsTests
{
    [Fact]
    public async Task Developer_exception_page_shows_the_exception_in_LocalDevelopment()
    {
        await using var factory = new WebFactory("LocalDevelopment");
        using var client = factory.CreateClient();

        var response = await client.GetAsync(new Uri(WebFactory.ThrowingPath, UriKind.Relative), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Contains(WebFactory.FakeFailureMessage, await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Production_shows_the_error_page_without_exception_details()
    {
        await using var factory = new WebFactory("Production");
        using var client = factory.CreateClient();

        var response = await client.GetAsync(new Uri(WebFactory.ThrowingPath, UriKind.Relative), TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.DoesNotContain(WebFactory.FakeFailureMessage, body, StringComparison.Ordinal);
        Assert.Contains("Sorry, there is a problem with the service", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task User_secrets_load_in_LocalDevelopment_before_environment_variables()
    {
        await using var factory = new WebFactory("LocalDevelopment");
        var providers = ((IConfigurationRoot)factory.Services.GetRequiredService<IConfiguration>()).Providers.ToList();

        var secrets = providers.FindIndex(p => p is JsonConfigurationProvider { Source.Path: "secrets.json" });
        var environmentSettings = providers.FindIndex(p => p is JsonConfigurationProvider { Source.Path: "appsettings.LocalDevelopment.json" });
        var environmentVariables = providers.FindLastIndex(p => p is EnvironmentVariablesConfigurationProvider);

        Assert.InRange(secrets, environmentSettings + 1, environmentVariables - 1);
    }

    [Fact]
    public async Task User_secrets_do_not_load_in_Production()
    {
        await using var factory = new WebFactory("Production");
        var providers = ((IConfigurationRoot)factory.Services.GetRequiredService<IConfiguration>()).Providers;

        Assert.DoesNotContain(providers, p => p is JsonConfigurationProvider { Source.Path: "secrets.json" });
    }

    [Fact]
    public async Task Service_registrations_are_validated_at_startup_in_LocalDevelopment()
    {
        await using var factory = new WebFactory("LocalDevelopment", services =>
        {
            services.AddScoped<ScopedDependency>();
            services.AddSingleton<SingletonCapturingScoped>();
        });

        Assert.ThrowsAny<Exception>(() => factory.CreateClient());
    }

    private sealed class ScopedDependency;

    private sealed class SingletonCapturingScoped(ScopedDependency dependency)
    {
        public ScopedDependency Dependency { get; } = dependency;
    }
}
