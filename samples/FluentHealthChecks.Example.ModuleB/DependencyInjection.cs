using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FluentHealthChecks.Example.ModuleB;

public static class DependencyInjection
{
    public static IServiceCollection AddExampleModuleB(this IServiceCollection services)
    {
        // Add module health checks
        services.AddFluentHealthChecks();
        return services;
    }

    public static void UseExampleModuleB(this WebApplication app)
    {
    }
}