namespace TodoList.Api.Tests.Infrastructure;

/// <summary>
/// xUnit collection definition that lets every test class participating in the
/// <c>"Integration"</c> collection share a single <see cref="IntegrationFixture"/>.
///
/// Tests within this collection are run sequentially (not in parallel as they do by default),
/// which is what we want when they share one SQL database that is reset between tests.
/// </summary>
[CollectionDefinition(Name)]
public sealed class IntegrationCollection : ICollectionFixture<IntegrationFixture>
{
    public const string Name = "Integration";
}
