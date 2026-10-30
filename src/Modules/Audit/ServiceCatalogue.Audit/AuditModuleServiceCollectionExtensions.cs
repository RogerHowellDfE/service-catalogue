using Microsoft.Extensions.DependencyInjection;

namespace ServiceCatalogue.Audit;

public static class AuditModuleServiceCollectionExtensions
{
    /// <summary>Registers the Audit module. Hosts call this; nothing else references the module's internals (ADR-0002).</summary>
    public static IServiceCollection AddAuditModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services;
    }
}
