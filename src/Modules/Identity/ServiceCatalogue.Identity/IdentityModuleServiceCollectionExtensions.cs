using Microsoft.Extensions.DependencyInjection;

namespace ServiceCatalogue.Identity;

public static class IdentityModuleServiceCollectionExtensions
{
    /// <summary>Registers the Identity module. Hosts call this; nothing else references the module's internals (ADR-0002).</summary>
    public static IServiceCollection AddIdentityModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services;
    }
}
