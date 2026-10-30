using Microsoft.Extensions.Hosting;

namespace ServiceCatalogue.SharedKernel;

/// <summary>
/// The only sanctioned way to branch on the environment (NFR-D4). <c>IsDevelopment()</c> is banned because
/// <c>Development</c> is a deployed environment here, not a developer's machine.
/// </summary>
public static class HostEnvironmentExtensions
{
    public const string LocalDevelopment = "LocalDevelopment";

    /// <summary>A developer's machine or CI: the only place security-relevant fakes are allowed.</summary>
    public static bool IsLocalDevelopment(this IHostEnvironment environment) => false;

    /// <summary><c>LocalDevelopment</c> or the deployed <c>Development</c> environment.</summary>
    public static bool IsDevEquivalent(this IHostEnvironment environment) => false;
}
