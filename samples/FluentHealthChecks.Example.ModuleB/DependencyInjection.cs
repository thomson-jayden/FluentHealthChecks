using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FluentHealthChecks.Example.ModuleB;

public static class DependencyInjection
{
    public static IServiceCollection AddExampleModuleB(this IServiceCollection services)
    {
        // Add module health checks
        services.AddHealthChecks()
            .AddCheck(
                "selfB",
                () => HealthCheckResult.Healthy(),
                tags: ["live"])
            .AddCheck(
                "readyB",
                () => HealthCheckResult.Healthy(),
                tags: ["ready"]);
        return services;
    }

    public static void UseExampleModuleB(this WebApplication app)
    {
    }
}