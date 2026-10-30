using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using ServiceCatalogue.Testing;

namespace ServiceCatalogue.Web.IntegrationTests;

public class LocalDevelopmentDefaultsTests
{
    private const string DevelopmentDefaults =
        "Defaults ASP.NET Core only switches on in Development are switched on in LocalDevelopment too, and never in deployed environments. " +
        "See docs/adr/0010-unconfigured-integrations-fall-back-safely.md (Decision) and NFR-D4 in docs/requirements.md.";

    private const string LocalConfigurationOnTheAppHost =
        "Local configuration has one home, the AppHost's user-secrets; Web and Worker do not load user-secrets. " +
        "See docs/adr/0010-unconfigured-integrations-fall-back-safely.md (Decision) and README.md (Getting started).";

    [Fact]
    public async Task Developer_exception_page_shows_the_exception_in_LocalDevelopment()
    {
        await using var factory = new WebFactory("LocalDevelopment");
        using var client = factory.CreateClient();

        var response = await client.GetAsync(new Uri(WebFactory.ThrowingPath, UriKind.Relative), TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Signpost.Check(DevelopmentDefaults, () =>
        {
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Contains(WebFactory.FakeFailureMessage, body, StringComparison.Ordinal);
        });
    }

    [Fact]
    public async Task Production_shows_the_error_page_without_exception_details()
    {
        await using var factory = new WebFactory("Production");
        using var client = factory.CreateClient();

        var response = await client.GetAsync(new Uri(WebFactory.ThrowingPath, UriKind.Relative), TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Signpost.Check(DevelopmentDefaults, () =>
        {
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.DoesNotContain(WebFactory.FakeFailureMessage, body, StringComparison.Ordinal);
            Assert.Contains("Sorry, there is a problem with the service", body, StringComparison.Ordinal);
        });
    }

    [Theory]
    [InlineData("LocalDevelopment")]
    [InlineData("Development")]
    [InlineData("Production")]
    public async Task User_secrets_are_not_loaded_because_local_configuration_lives_on_the_AppHost(string environment)
    {
        await using var factory = new WebFactory(environment);
        var configuration = (IConfigurationRoot)factory.Services.GetRequiredService<IConfiguration>();

        Signpost.Check(LocalConfigurationOnTheAppHost, () =>
            Assert.DoesNotContain(configuration.Providers, p => p is JsonConfigurationProvider { Source.Path: "secrets.json" }));
    }

    [Fact]
    public async Task Service_registrations_are_validated_at_startup_in_LocalDevelopment()
    {
        await using var factory = new WebFactory("LocalDevelopment", services =>
        {
            services.AddScoped<ScopedDependency>();
            services.AddSingleton<SingletonCapturingScoped>();
        });

        Signpost.Check(DevelopmentDefaults, () =>
            Assert.ThrowsAny<Exception>(() => factory.CreateClient()));
    }

    private sealed class ScopedDependency;

    private sealed class SingletonCapturingScoped(ScopedDependency dependency)
    {
        public ScopedDependency Dependency { get; } = dependency;
    }
}
