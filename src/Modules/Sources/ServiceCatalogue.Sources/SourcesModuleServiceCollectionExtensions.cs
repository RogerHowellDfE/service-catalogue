using Microsoft.Extensions.DependencyInjection;

namespace ServiceCatalogue.Sources;

public static class SourcesModuleServiceCollectionExtensions
{
    /// <summary>Registers the Sources module. Hosts call this; nothing else references the module's internals (ADR-0002).</summary>
    public static IServiceCollection AddSourcesModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services;
    }
}
