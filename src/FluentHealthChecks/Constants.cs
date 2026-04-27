namespace FluentHealthChecks;

public static class Constants
{
    // Tags
    public const string LiveTag = "live";
    public const string ReadyTag = "ready";

    // Endpoints
    public const string LiveEndpoint = "health/live";
    public const string ReadyEndpoint = "health/ready";
    public const string HealthEndpoint = "health";
}