using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
namespace LastMile.TMS.Api.Tests.Controllers;

/// <summary>
/// Integration tests for the OpenIddict /connect/token endpoint.
/// These are the TDD Red tests — they define the expected behaviour before
/// implementation was complete, then turn Green once everything is wired.
/// </summary>
public class AuthTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        BaseAddress = new Uri("https://localhost")
    });

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => Task.CompletedTask;

    // ── Helpers ────────────────────────────────────────────────────────────────

    private static FormUrlEncodedContent PasswordGrantBody(string username, string password) =>
        new([
            new("grant_type", "password"),
            new("username", username),
            new("password", password)
        ]);

    // ── Tests ──────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Login_WithValidAdminCredentials_Returns200AndTokens()
    {
        // Arrange
        var body = PasswordGrantBody("admin@lastmile.com", "Admin@12345");

        // Act
        var response = await _client.PostAsync("/connect/token", body);

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        var json = await ParseJsonAsync(response);
        json.Should().ContainKey("access_token");
        json.Should().ContainKey("token_type");
        json["token_type"]!.GetString().Should().BeEquivalentTo("Bearer");
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsBadRequest()
    {
        // Arrange
        var body = PasswordGrantBody("admin@lastmile.com", "WrongPassword!");

        // Act
        var response = await _client.PostAsync("/connect/token", body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithUnknownUser_ReturnsBadRequest()
    {
        // Arrange
        var body = PasswordGrantBody("nobody@example.com", "Admin@12345");

        // Act
        var response = await _client.PostAsync("/connect/token", body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_Returns401()
    {
        // Arrange — hit a controller action decorated with [Authorize]
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/api/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // ── Private helpers ────────────────────────────────────────────────────────

    private static async Task<Dictionary<string, JsonElement>> ParseJsonAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(content)!;
    }
}
