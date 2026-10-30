using Microsoft.Extensions.DependencyInjection;

namespace ServiceCatalogue.Insights;

public static class InsightsModuleServiceCollectionExtensions
{
    /// <summary>Registers the Insights module. Hosts call this; nothing else references the module's internals (ADR-0002).</summary>
    public static IServiceCollection AddInsightsModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services;
    }
}
