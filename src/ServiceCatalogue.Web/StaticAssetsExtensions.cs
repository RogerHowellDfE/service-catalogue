using ServiceCatalogue.SharedKernel;

namespace ServiceCatalogue.Web;

public static class StaticAssetsExtensions
{
    /// <summary>
    /// In LocalDevelopment, serves wwwroot and package assets from the project folders, as the host only does by itself in Development.
    /// Elsewhere does nothing: published output already contains the assets.
    /// </summary>
    public static WebApplicationBuilder AddStaticAssets(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (builder.Environment.IsLocalDevelopment())
        {
            builder.WebHost.UseStaticWebAssets();
        }

        return builder;
    }
}
