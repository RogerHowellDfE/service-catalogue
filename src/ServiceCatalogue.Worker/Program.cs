using ServiceCatalogue.Audit;
using ServiceCatalogue.Catalogue;
using ServiceCatalogue.Identity;
using ServiceCatalogue.Insights;
using ServiceCatalogue.ServiceDefaults;
using ServiceCatalogue.SharedKernel;
using ServiceCatalogue.Sources;

// A web host only so it can expose health endpoints; background work arrives with the features that need it.
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

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();
