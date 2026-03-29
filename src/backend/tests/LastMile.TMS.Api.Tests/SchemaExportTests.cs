using System.Net;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace LastMile.TMS.Api.Tests;

/// <summary>
/// Exports the GraphQL schema SDL to a file for frontend codegen.
/// Run: dotnet test --filter "SchemaExportTests"
/// </summary>
[Collection(ApiTestCollection.Name)]
public class SchemaExportTests(CustomWebApplicationFactory factory)
{
    [Fact]
    public async Task ExportSchema_ToFile()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/graphql/schema.graphql");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var sdl = await response.Content.ReadAsStringAsync();
        sdl.Should().NotBeEmpty();

        // Write schema to the web project root for codegen consumption
        // Walk up from bin/Debug/net10.0 → tests/LastMile.TMS.Api.Tests → tests → backend → src → repo root
        var binDir = AppContext.BaseDirectory;
        var repoRoot = Path.GetFullPath(Path.Combine(binDir, "..", "..", "..", "..", "..", ".."));
        var schemaPath = Path.Combine(repoRoot, "web", "schema.graphql");
        Console.WriteLine($"BaseDirectory: {binDir}");
        Console.WriteLine($"RepoRoot: {repoRoot}");
        await File.WriteAllTextAsync(schemaPath, sdl);
        Console.WriteLine($"Schema exported to: {schemaPath}");
    }
}
