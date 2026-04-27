using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FluentHealthChecks.AzureFunctions;

public class HealthCheckFunction(HealthCheckService healthCheckService)
{
    [Function(nameof(Live))]
    public async Task<IActionResult> Live(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health/live")] HttpRequest req)
    {
        var result = await healthCheckService.CheckHealthAsync(
            check => check.Tags.Contains(Constants.LiveTag));

        return StatusResult(result.Status);
    }

    [Function(nameof(Ready))]
    public async Task<IActionResult> Ready(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health/ready")] HttpRequest req)
    {
        var result = await healthCheckService.CheckHealthAsync(
            check => check.Tags.Contains(Constants.ReadyTag));

        return StatusResult(result.Status);
    }

    [Function(nameof(Health))]
    public async Task<IActionResult> Health(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequest req)
    {
        var result = await healthCheckService.CheckHealthAsync(
            check => check.Tags.Contains(Constants.LiveTag) || check.Tags.Contains(Constants.ReadyTag));

        return StatusResult(result.Status);
    }

    private static IActionResult StatusResult(HealthStatus status) =>
        status == HealthStatus.Healthy
            ? new OkObjectResult(new { status = nameof(HealthStatus.Healthy) })
            : new ObjectResult(new { status = status.ToString() })
            {
                StatusCode = StatusCodes.Status503ServiceUnavailable
            };
}

