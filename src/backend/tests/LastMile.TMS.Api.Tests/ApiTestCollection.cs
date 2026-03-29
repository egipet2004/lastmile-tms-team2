namespace LastMile.TMS.Api.Tests;

[CollectionDefinition(Name)]
public sealed class ApiTestCollection : ICollectionFixture<CustomWebApplicationFactory>
{
    public const string Name = "ApiTests";
}
