using FluentHealthChecks.AzureFunctions;
using Microsoft.Extensions.DependencyInjection;

namespace FluentHealthChecks.DependencyInjection;

public static class FunctionAppDependencyInjection
{
    /// <summary>
    /// Registers the <see cref="HealthCheckFunction"/> with the DI container so the
    /// Azure Functions runtime can resolve it and expose the /health, /health/live,
    /// and /health/ready endpoints automatically.
    /// </summary>
    public static IServiceCollection AddFluentHealthCheckFunctions(this IServiceCollection services)
    {
        services.AddSingleton<HealthCheckFunction>();
        return services;
    }
}