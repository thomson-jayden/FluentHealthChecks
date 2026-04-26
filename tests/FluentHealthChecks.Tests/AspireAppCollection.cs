namespace FluentHealthChecks.Tests;

[CollectionDefinition("AspireApp", DisableParallelization = true)]
public sealed class AspireAppCollection : ICollectionFixture<AspireAppFixture>
{
}