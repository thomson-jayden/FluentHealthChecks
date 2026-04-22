var builder = DistributedApplication.CreateBuilder(args);

_ = builder
    .AddProject<Projects.FluentHealthChecks_Example_Api>("api")
    .WithExternalHttpEndpoints();

builder.Build().Run();