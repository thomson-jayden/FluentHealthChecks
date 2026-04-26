using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;

namespace FluentHealthChecks.Tests;

public sealed class AspireAppFixture : IAsyncLifetime
{
    private IAsyncDisposable? _app;
    private Uri? _apiBaseAddress;
    private Uri? _functionsBaseAddress;

    public async Task InitializeAsync()
    {
        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.FluentHealthChecks_AppHost>();
        var azuriteResourceName = GetAzuriteResourceName(builder);

        var app = await builder.BuildAsync();
        await app.StartAsync();

        if (azuriteResourceName is not null)
        {
            await WaitForResourceHealthyAsync(app, azuriteResourceName, TimeSpan.FromSeconds(30));
        }

        await WaitForResourceStateAsync(app, "api", KnownResourceStates.Running, TimeSpan.FromSeconds(30));
        await WaitForResourceStateAsync(app, "functions", KnownResourceStates.Running, TimeSpan.FromSeconds(30));

        _apiBaseAddress = app.GetEndpoint("api", "https");
        _functionsBaseAddress = app.GetEndpoint("functions", "http");

        await WaitForEndpointAsync(_apiBaseAddress, "/health/live", TimeSpan.FromSeconds(30), allowInvalidCertificate: true);
        await WaitForEndpointAsync(_functionsBaseAddress, "/api/health/live", TimeSpan.FromSeconds(30));

        _app = app;
    }

    public async Task DisposeAsync()
    {
        if (_app is not null)
        {
            await _app.DisposeAsync();
        }
    }

    public HttpClient CreateApiClient()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        return new HttpClient(handler)
        {
            BaseAddress = _apiBaseAddress ?? throw new InvalidOperationException("Aspire app is not initialized.")
        };
    }

    public HttpClient CreateFunctionsClient()
    {
        return new HttpClient
        {
            BaseAddress = _functionsBaseAddress ?? throw new InvalidOperationException("Aspire app is not initialized.")
        };
    }

    private static string? GetAzuriteResourceName(IDistributedApplicationTestingBuilder builder)
    {
        return builder.Resources
            .Where(static resource =>
            {
                var typeName = resource.GetType().FullName;

                return typeName is not null &&
                    (typeName.Contains("AzureStorageEmulatorResource", StringComparison.Ordinal) ||
                     typeName.Contains("AzureStorageResource", StringComparison.Ordinal));
            })
            .Select(static resource => resource.Name)
            .FirstOrDefault()
            ?? builder.Resources
                .Select(static resource => resource.Name)
                .FirstOrDefault(static name => name.StartsWith("funcstorage", StringComparison.OrdinalIgnoreCase));
    }

    private static async Task WaitForResourceHealthyAsync(DistributedApplication app, string resourceName, TimeSpan timeout)
    {
        using var timeoutTokenSource = new CancellationTokenSource(timeout);

        try
        {
            await app.ResourceNotifications.WaitForResourceHealthyAsync(resourceName, timeoutTokenSource.Token);
        }
        catch (OperationCanceledException ex) when (timeoutTokenSource.IsCancellationRequested)
        {
            throw new TimeoutException($"Resource '{resourceName}' did not become healthy within {timeout}.", ex);
        }
    }

    private static async Task WaitForResourceStateAsync(DistributedApplication app, string resourceName, string state, TimeSpan timeout)
    {
        using var timeoutTokenSource = new CancellationTokenSource(timeout);

        try
        {
            await app.ResourceNotifications.WaitForResourceAsync(resourceName, state, timeoutTokenSource.Token);
        }
        catch (OperationCanceledException ex) when (timeoutTokenSource.IsCancellationRequested)
        {
            throw new TimeoutException($"Resource '{resourceName}' did not reach state '{state}' within {timeout}.", ex);
        }
    }

    private static async Task WaitForEndpointAsync(Uri baseAddress, string relativePath, TimeSpan timeout, bool allowInvalidCertificate = false)
    {
        using var timeoutTokenSource = new CancellationTokenSource(timeout);
        using var client = CreateProbeClient(baseAddress, allowInvalidCertificate);

        while (!timeoutTokenSource.IsCancellationRequested)
        {
            try
            {
                using var response = await client.GetAsync(relativePath, timeoutTokenSource.Token);

                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch (HttpRequestException)
            {
                // The server is not accepting requests yet; retry until the timeout expires.
            }
            catch (TaskCanceledException) when (!timeoutTokenSource.IsCancellationRequested)
            {
                // Individual probe timed out before the overall readiness timeout; retry.
            }

            await Task.Delay(TimeSpan.FromMilliseconds(250), timeoutTokenSource.Token);
        }

        throw new TimeoutException($"Endpoint '{new Uri(baseAddress, relativePath)}' did not return success within {timeout}.");
    }

    private static HttpClient CreateProbeClient(Uri baseAddress, bool allowInvalidCertificate)
    {
        if (!allowInvalidCertificate)
        {
            return new HttpClient { BaseAddress = baseAddress };
        }

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        return new HttpClient(handler)
        {
            BaseAddress = baseAddress
        };
    }
}