using FluentAssertions;
using LastMile.TMS.Api.Configuration;
using Microsoft.Extensions.Configuration;

namespace LastMile.TMS.Api.Tests.Configuration;

public class HangfireDashboardConfigurationTests
{
    [Fact]
    public void CreateOptions_WithCredentials_AddsAuthorizationFilter()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Dashboard:Username"] = "admin",
                ["Dashboard:Password"] = "devpass123"
            })
            .Build();

        var options = HangfireDashboardConfiguration.CreateOptions(configuration);

        options.Authorization.Should().NotBeNull();
        options.Authorization.Should().ContainSingle();
    }

    [Fact]
    public void CreateOptions_WithoutCredentials_UsesDefaultLocalAuthorization()
    {
        var configuration = new ConfigurationBuilder().Build();

        var options = HangfireDashboardConfiguration.CreateOptions(configuration);

        options.Authorization.Should().ContainSingle();
        options.Authorization!.Single().GetType().Name.Should().Be("LocalRequestsOnlyAuthorizationFilter");
    }

    [Fact]
    public void TryValidateBasicAuthHeader_WithValidCredentials_ReturnsTrue()
    {
        var header = $"Basic {Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("admin:devpass123"))}";

        var result = HangfireDashboardConfiguration.TryValidateBasicAuthHeader(
            header,
            "admin",
            "devpass123");

        result.Should().BeTrue();
    }

    [Fact]
    public void TryValidateBasicAuthHeader_WithInvalidPassword_ReturnsFalse()
    {
        var header = $"Basic {Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("admin:wrong"))}";

        var result = HangfireDashboardConfiguration.TryValidateBasicAuthHeader(
            header,
            "admin",
            "devpass123");

        result.Should().BeFalse();
    }

    [Fact]
    public void TryValidateBasicAuthHeader_WithMalformedHeader_ReturnsFalse()
    {
        var result = HangfireDashboardConfiguration.TryValidateBasicAuthHeader(
            "Basic not-base64",
            "admin",
            "devpass123");

        result.Should().BeFalse();
    }
}
