using FluentHealthChecks.AzureFunctions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FluentHealthChecks;

public static class DependencyInjection
{
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