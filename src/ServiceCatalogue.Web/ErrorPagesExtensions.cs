using ServiceCatalogue.SharedKernel;

namespace ServiceCatalogue.Web;

public static class ErrorPagesExtensions
{
    /// <summary>
    /// Developer exception page in LocalDevelopment; the /Error page and HSTS everywhere else.
    /// Call first, so it catches exceptions from everything after it.
    /// </summary>
    public static WebApplication UseErrorPages(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        if (app.Environment.IsLocalDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        return app;
    }
}
