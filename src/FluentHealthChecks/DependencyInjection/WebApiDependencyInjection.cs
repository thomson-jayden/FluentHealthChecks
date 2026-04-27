using Microsoft.AspNetCore.Builder;

namespace FluentHealthChecks.DependencyInjection;

public static class WebApiDependencyInjection
{
    public static void UseFluentHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health/live", new()
        {
            Predicate = check => check.Tags.Contains(Constants.LiveTag)
        });

        app.MapHealthChecks("/health/ready", new()
        {
            Predicate = check => check.Tags.Contains(Constants.ReadyTag)
        });

        // Keep /health as a combined endpoint for both liveness and readiness.
        app.MapHealthChecks("/health", new()
        {
            Predicate = check => check.Tags.Contains(Constants.LiveTag) || check.Tags.Contains(Constants.ReadyTag)
        });
    }
}