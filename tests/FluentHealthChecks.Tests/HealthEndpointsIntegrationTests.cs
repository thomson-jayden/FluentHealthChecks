namespace FluentHealthChecks.Tests;

[Collection("AspireApp")]
public class HealthEndpointsIntegrationTests
{
    private readonly AspireAppFixture _aspireApp;

    public HealthEndpointsIntegrationTests(AspireAppFixture aspireApp)
    {
        _aspireApp = aspireApp;
    }

    [Theory]
    [InlineData("/health/live")]
    [InlineData("/health/ready")]
    [InlineData("/health")]
    public async Task Api_health_endpoints_return_success(string path)
    {
        using var client = _aspireApp.CreateApiClient();
        using var response = await client.GetAsync(path);

        Assert.True(
            response.IsSuccessStatusCode,
            $"Expected 2xx for API path '{path}' but got {(int)response.StatusCode} ({response.StatusCode}).");
    }

    [Theory]
    [InlineData("/api/health/live")]
    [InlineData("/api/health/ready")]
    [InlineData("/api/health")]
    public async Task Function_app_health_endpoints_return_success(string path)
    {
        using var client = _aspireApp.CreateFunctionsClient();
        using var response = await client.GetAsync(path);

        Assert.True(
            response.IsSuccessStatusCode,
            $"Expected 2xx for Function App path '{path}' but got {(int)response.StatusCode} ({response.StatusCode}).");
    }
}