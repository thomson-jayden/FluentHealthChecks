using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FluentHealthChecks.Example.ModuleA;

public static class DependencyInjection
{
    public static IServiceCollection AddExampleModuleA(this IServiceCollection services)
    {
        // Add module health checks
        services.AddFluentHealthChecks();
        return services;
    }

    public static void UseExampleModuleA(this WebApplication app)
    {
    }
}