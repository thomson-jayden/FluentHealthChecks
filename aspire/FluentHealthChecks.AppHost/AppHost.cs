var builder = DistributedApplication.CreateBuilder(args);

_ = builder
    .AddProject<Projects.FluentHealthChecks_Example_Api>("api")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithUrl("/health", "Health");

builder.Build().Run();