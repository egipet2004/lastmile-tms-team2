using System.Security.Claims;
using LastMile.TMS.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace LastMile.TMS.Api.Controllers;

[ApiController]
[Route("connect")]
public class AuthController(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpPost("token")]
    [Consumes("application/x-www-form-urlencoded")]
    [Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        if (request?.IsPasswordGrantType() ?? false)
        {
            var user = await userManager.FindByNameAsync(request.Username!);
            if (user == null)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The username/password couple is invalid."
                    }));
            }

            // Ensure the user is allowed to sign in.
            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password!, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The username/password couple is invalid."
                    }));
            }

            // Create the identity claims
            var identity = new ClaimsIdentity(
                authenticationType: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                nameType: ClaimTypes.Name,
                roleType: ClaimTypes.Role);

            // Add basic claims
            identity.AddClaim(new Claim(OpenIddictConstants.Claims.Subject, user.Id.ToString())
                .SetDestinations(OpenIddictConstants.Destinations.AccessToken));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Email!)
                .SetDestinations(OpenIddictConstants.Destinations.AccessToken));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email!)
                .SetDestinations(OpenIddictConstants.Destinations.AccessToken));

            // Roles
            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role)
                    .SetDestinations(OpenIddictConstants.Destinations.AccessToken));
            }

            var principal = new ClaimsPrincipal(identity);
            
            // Set required scopes (like offline_access for refresh tokens)
            principal.SetScopes(request.GetScopes());

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        throw new NotImplementedException("The specified grant type is not implemented.");
    }
}
