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

        await using var app = await builder.BuildAsync();
        await app.StartAsync();

        using var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        using var client = new HttpClient(handler)
        {
            BaseAddress = app.GetEndpoint("api", "https")
        };
        using var response = await client.GetAsync(path);

        Assert.True(
            response.IsSuccessStatusCode,
            $"Expected 2xx for '{path}' but got {(int)response.StatusCode} ({response.StatusCode}).");
    }
}