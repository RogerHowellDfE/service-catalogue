using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace ServiceCatalogue.SharedKernel.UnitTests;

public class HostEnvironmentExtensionsTests
{
    [Theory]
    [InlineData("LocalDevelopment")]
    [InlineData("localdevelopment")]
    [InlineData("LOCALDEVELOPMENT")]
    public void IsLocalDevelopment_is_true_for_LocalDevelopment_in_any_case(string name)
    {
        Assert.True(new FakeEnvironment(name).IsLocalDevelopment());
    }

    [Theory]
    [InlineData("Development")]
    [InlineData("Test")]
    [InlineData("Production")]
    [InlineData("")]
    [InlineData("Local Development")]
    [InlineData("LocalDevelopment ")]
    [InlineData("LocalDevelopmentX")]
    public void IsLocalDevelopment_is_false_for_anything_else(string name)
    {
        Assert.False(new FakeEnvironment(name).IsLocalDevelopment());
    }

    [Theory]
    [InlineData("LocalDevelopment")]
    [InlineData("localdevelopment")]
    [InlineData("Development")]
    [InlineData("development")]
    public void IsDevEquivalent_is_true_for_LocalDevelopment_and_Development(string name)
    {
        Assert.True(new FakeEnvironment(name).IsDevEquivalent());
    }

    [Theory]
    [InlineData("Test")]
    [InlineData("Staging")]
    [InlineData("Production")]
    [InlineData("")]
    public void IsDevEquivalent_is_false_for_other_environments(string name)
    {
        Assert.False(new FakeEnvironment(name).IsDevEquivalent());
    }

    [Fact]
    public void Helpers_reject_a_null_environment()
    {
        IHostEnvironment environment = null!;

        Assert.Throws<ArgumentNullException>(() => environment.IsLocalDevelopment());
        Assert.Throws<ArgumentNullException>(() => environment.IsDevEquivalent());
    }

    private sealed class FakeEnvironment(string name) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = name;
        public string ApplicationName { get; set; } = "Fake";
        public string ContentRootPath { get; set; } = "";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
