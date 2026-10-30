using Microsoft.Extensions.DependencyInjection;

namespace ServiceCatalogue.SharedKernel.UnitTests;

public class SharedKernelServiceCollectionExtensionsTests
{
    [Fact]
    public void AddSharedKernel_registers_the_system_TimeProvider()
    {
        using var provider = new ServiceCollection().AddSharedKernel().BuildServiceProvider();

        Assert.Same(TimeProvider.System, provider.GetService<TimeProvider>());
    }

    [Fact]
    public void AddSharedKernel_keeps_a_TimeProvider_that_is_already_registered()
    {
        var fake = new FixedTimeProvider();
        using var provider = new ServiceCollection()
            .AddSingleton<TimeProvider>(fake)
            .AddSharedKernel()
            .BuildServiceProvider();

        Assert.Same(fake, provider.GetService<TimeProvider>());
    }

    private sealed class FixedTimeProvider : TimeProvider;
}
