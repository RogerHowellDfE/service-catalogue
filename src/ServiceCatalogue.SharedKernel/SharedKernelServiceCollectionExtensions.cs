using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ServiceCatalogue.SharedKernel;

public static class SharedKernelServiceCollectionExtensions
{
    /// <summary>Registers <see cref="TimeProvider.System"/> unless a <see cref="TimeProvider"/> (for example a fake in tests) is already registered (NFR-V4).</summary>
    public static IServiceCollection AddSharedKernel(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.TryAddSingleton(TimeProvider.System);
        return services;
    }
}
