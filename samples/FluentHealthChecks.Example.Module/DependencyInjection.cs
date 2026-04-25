using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FluentHealthChecks.Example.Module;

public static class DependencyInjection
{
    public static void AddExampleModule(this IServiceCollection services)
    {
        // Add module health checks
        services.AddHealthChecks()
            .AddCheck(
                "self",
                () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(),
                tags: ["live"])
            .AddCheck(
                "ready",
                () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(),
                tags: ["ready"]);
    }

    public static void UseExampleModule(this WebApplication app)
    {
    }
}