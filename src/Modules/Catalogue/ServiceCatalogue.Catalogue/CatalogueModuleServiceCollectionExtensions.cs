using Microsoft.Extensions.DependencyInjection;

namespace ServiceCatalogue.Catalogue;

public static class CatalogueModuleServiceCollectionExtensions
{
    /// <summary>Registers the Catalogue module. Hosts call this; nothing else references the module's internals (ADR-0002).</summary>
    public static IServiceCollection AddCatalogueModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services;
    }
}
