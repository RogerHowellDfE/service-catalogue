using System.Net;

namespace ServiceCatalogue.Web.IntegrationTests;

public class SkeletonTests
{
    [Fact]
    public async Task Home_page_renders_the_placeholder()
    {
        await using var factory = new WebFactory("LocalDevelopment");
        using var client = factory.CreateClient();

        var response = await client.GetAsync(new Uri("/", UriKind.Relative), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("<h1>Service Catalogue</h1>", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken), StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("LocalDevelopment")]
    [InlineData("localdevelopment")]
    [InlineData("Development")]
    public async Task Alive_endpoint_is_mapped_in_dev_equivalent_environments(string environment)
    {
        await using var factory = new WebFactory(environment);
        using var client = factory.CreateClient();

        var response = await client.GetAsync(new Uri("/alive", UriKind.Relative), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Health_endpoint_reports_unhealthy_when_the_database_is_unreachable()
    {
        await using var factory = new WebFactory("LocalDevelopment");
        using var client = factory.CreateClient();

        var response = await client.GetAsync(new Uri("/health", UriKind.Relative), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Theory]
    [InlineData("/alive")]
    [InlineData("/health")]
    public async Task Health_endpoints_are_not_mapped_in_Production(string path)
    {
        await using var factory = new WebFactory("Production");
        using var client = factory.CreateClient();

        var response = await client.GetAsync(new Uri(path, UriKind.Relative), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
