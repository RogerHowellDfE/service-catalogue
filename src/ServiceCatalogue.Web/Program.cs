using ServiceCatalogue.Audit;
using ServiceCatalogue.Catalogue;
using ServiceCatalogue.Identity;
using ServiceCatalogue.Insights;
using ServiceCatalogue.SharedKernel;
using ServiceCatalogue.Sources;
using ServiceCatalogue.Web;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDataSource("servicecatalogue");

builder.Services
    .AddSharedKernel()
    .AddSourcesModule()
    .AddCatalogueModule()
    .AddInsightsModule()
    .AddIdentityModule()
    .AddAuditModule();
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseErrorPages();
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.MapDefaultEndpoints();

app.Run();
