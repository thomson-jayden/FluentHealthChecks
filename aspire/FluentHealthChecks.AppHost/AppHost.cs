var builder = DistributedApplication.CreateBuilder(args);

_ = builder
    .AddProject<Projects.FluentHealthChecks_Example_Api>("api")
    .WithUrlForEndpoint("https", _ => new() { Url = "/health/live", DisplayText = "Live" })
    .WithUrlForEndpoint("https", _ => new() { Url = "/health/ready", DisplayText = "Ready" })
    ;

builder.Build().Run();