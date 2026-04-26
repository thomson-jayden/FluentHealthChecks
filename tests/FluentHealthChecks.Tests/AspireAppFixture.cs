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

        var app = await builder.BuildAsync();
        try
        {
            await app.StartAsync();

            _apiBaseAddress = app.GetEndpoint("api", "https");
            _functionsBaseAddress = app.GetEndpoint("functions", "http");

            await WaitForEndpointAsync(_apiBaseAddress, "/health/live", TimeSpan.FromSeconds(30), allowInvalidCertificate: true);
            await WaitForEndpointAsync(_functionsBaseAddress, "/api/health/live", TimeSpan.FromSeconds(30));

            _app = app;
        }
        catch
        {
            await app.DisposeAsync();
            throw;
        }
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
            catch (TaskCanceledException) when (timeoutTokenSource.IsCancellationRequested)
            {
                break;
            }

            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(250), timeoutTokenSource.Token);
            }
            catch (OperationCanceledException) when (timeoutTokenSource.IsCancellationRequested)
            {
                break;
            }
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