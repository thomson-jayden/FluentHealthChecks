using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FluentHealthChecks.Example.ModuleA;

public static class DependencyInjection
{
    public static IServiceCollection AddExampleModuleA(this IServiceCollection services)
    {
        // Add module health checks
        services.AddHealthChecks()
            .AddCheck(
                "selfA",
                () => HealthCheckResult.Healthy(),
                tags: ["live"])
            .AddCheck(
                "readyA",
                () => HealthCheckResult.Healthy(),
                tags: ["ready"]);
        return services;
    }

    public static void UseExampleModuleA(this WebApplication app)
    {
    }
}