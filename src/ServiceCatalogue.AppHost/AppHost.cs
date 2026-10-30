using ServiceCatalogue.SharedKernel;

var builder = DistributedApplication.CreateBuilder(args);

// Aspire generates the password; nothing is committed (NFR-S4).
var database = builder.AddPostgres("postgres")
    .AddDatabase("servicecatalogue");

builder.AddProject<Projects.ServiceCatalogue_Web>("web")
    .WithReference(database)
    .WaitFor(database)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", HostEnvironmentExtensions.LocalDevelopment)
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.ServiceCatalogue_Worker>("worker")
    .WithReference(database)
    .WaitFor(database)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", HostEnvironmentExtensions.LocalDevelopment)
    .WithHttpHealthCheck("/health");

builder.Build().Run();
