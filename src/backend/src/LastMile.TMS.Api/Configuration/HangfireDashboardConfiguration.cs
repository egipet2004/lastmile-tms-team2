using System.Security.Cryptography;
using System.Text;
using Hangfire;
using Hangfire.AspNetCore;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace LastMile.TMS.Api.Configuration;

public static class HangfireDashboardConfiguration
{
    public static DashboardOptions CreateOptions(IConfiguration configuration)
    {
        var username = configuration["Dashboard:Username"];
        var password = configuration["Dashboard:Password"];

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return new DashboardOptions();
        }

        return new DashboardOptions
        {
            Authorization = [new BasicAuthDashboardAuthorizationFilter(username, password)]
        };
    }

    public static bool TryValidateBasicAuthHeader(
        string? authorizationHeader,
        string expectedUsername,
        string expectedPassword)
    {
        if (string.IsNullOrWhiteSpace(authorizationHeader) ||
            !authorizationHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        try
        {
            var encodedCredentials = authorizationHeader["Basic ".Length..].Trim();
            var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
            var separatorIndex = decodedCredentials.IndexOf(':');

            if (separatorIndex <= 0)
            {
                return false;
            }

            var providedUsername = decodedCredentials[..separatorIndex];
            var providedPassword = decodedCredentials[(separatorIndex + 1)..];

            return FixedTimeEquals(providedUsername, expectedUsername) &&
                FixedTimeEquals(providedPassword, expectedPassword);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static bool FixedTimeEquals(string left, string right)
    {
        var leftBytes = Encoding.UTF8.GetBytes(left);
        var rightBytes = Encoding.UTF8.GetBytes(right);
        return CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
    }

    private sealed class BasicAuthDashboardAuthorizationFilter(
        string username,
        string password) : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var authorizationHeader = httpContext.Request.Headers.Authorization.ToString();

            if (TryValidateBasicAuthHeader(authorizationHeader, username, password))
            {
                return true;
            }

            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            httpContext.Response.Headers.WWWAuthenticate = "Basic realm=\"Hangfire Dashboard\"";
            return false;
        }
    }
}
