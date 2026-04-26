using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FluentHealthChecks.Example.ModuleB;

public static class DependencyInjection
{
    public static IServiceCollection AddExampleModuleB(this IServiceCollection services)
    {
        // Add module health checks
        services.AddHealthChecks()
            .AddCheck(
                "selfB",
                () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(),
                tags: ["live"])
            .AddCheck(
                "readyB",
                () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(),
                tags: ["ready"]);
        return services;
    }

    public static void UseExampleModuleB(this WebApplication app)
    {
    }
}