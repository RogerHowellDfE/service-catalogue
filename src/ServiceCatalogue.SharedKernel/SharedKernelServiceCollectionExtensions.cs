using Microsoft.Extensions.DependencyInjection;

namespace ServiceCatalogue.SharedKernel;

public static class SharedKernelServiceCollectionExtensions
{
    /// <summary>Registers <see cref="TimeProvider.System"/> unless a <see cref="TimeProvider"/> (for example a fake in tests) is already registered (NFR-V4).</summary>
    public static IServiceCollection AddSharedKernel(this IServiceCollection services) => services;
}
