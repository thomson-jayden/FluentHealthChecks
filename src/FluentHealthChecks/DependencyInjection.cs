using FluentHealthChecks.AzureFunctions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FluentHealthChecks;

public static class DependencyInjection
{
    public static IHealthChecksBuilder AddFluentHealthChecks(this IServiceCollection services)
    {
        var healthChecksBuilder = services.AddHealthChecks()
            .AddCheck(
                $"self{Guid.NewGuid()}",
                () => HealthCheckResult.Healthy(),
                tags: [Constants.LiveTag])
            .AddCheck(
                $"ready{Guid.NewGuid()}",
                () => HealthCheckResult.Healthy(),
                tags: [Constants.ReadyTag]);
        
        return healthChecksBuilder;
    }
    
    public static WebApplication UseFluentHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks(Constants.LiveEndpoint, new()
        {
            Predicate = check => check.Tags.Contains(Constants.LiveTag)
        });

        app.MapHealthChecks(Constants.ReadyEndpoint, new()
        {
            Predicate = check => check.Tags.Contains(Constants.ReadyTag)
        });

        // Keep /health as a combined endpoint for both liveness and readiness.
        app.MapHealthChecks(Constants.HealthEndpoint, new()
        {
            Predicate = check => check.Tags.Contains(Constants.LiveTag) || check.Tags.Contains(Constants.ReadyTag)
        });
        
        return app;
    }
    
    public static IServiceCollection UseFluentHealthChecks(this IServiceCollection services)
    {
        services.AddSingleton<HealthCheckFunction>();
        return services;
    }
}