using LastMile.TMS.Application.Common.Interfaces;
using LastMile.TMS.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LastMile.TMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Configure OpenIddict server (password grant → /connect/token)
        services.AddOpenIddict()
            .AddServer(options =>
            {
                // Enable password and refresh-token grant types
                options.AllowPasswordFlow()
                       .AllowRefreshTokenFlow();

                // Token endpoint
                options.SetTokenEndpointUris("/connect/token");

                // Accept anonymous clients (no client_id required for password flow)
                options.AcceptAnonymousClients();

                // Use JWT access tokens
                options.UseAspNetCore()
                       .EnableTokenEndpointPassthrough()
                       .DisableTransportSecurityRequirement();

                // Use ephemeral signing/encryption keys (OK for dev; swap for real certs in prod)
                options.AddEphemeralEncryptionKey()
                       .AddEphemeralSigningKey()
                       .DisableAccessTokenEncryption(); // plain JWT (not encrypted JWE)
            })
            .AddValidation(options =>
            {
                // Validate tokens issued by the local OpenIddict server
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        return services;
    }
}
