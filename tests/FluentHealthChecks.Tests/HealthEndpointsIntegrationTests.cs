using Aspire.Hosting.Testing;

namespace FluentHealthChecks.Tests;

public class HealthEndpointsIntegrationTests
{
    [Theory]
    [InlineData("/health/live")]
    [InlineData("/health/ready")]
    public async Task Health_endpoints_return_success(string path)
    {
        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.FluentHealthChecks_AppHost>();

        builder.Configuration["ASPIRE_ALLOW_UNSECURED_TRANSPORT"] = "true";

        await using var app = await builder.BuildAsync();
        await app.StartAsync();

        using var client = app.CreateHttpClient("api");
        using var response = await client.GetAsync(path);

        Assert.True(
            response.IsSuccessStatusCode,
            $"Expected 2xx for '{path}' but got {(int)response.StatusCode} ({response.StatusCode}).");
    }
}