using Microsoft.AspNetCore.Builder;

namespace FluentHealthChecks.DependencyInjection;

public static class WebApiDependencyInjection
{
    public static void UseFluentHealthChecks(this WebApplication app)
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
    }
}