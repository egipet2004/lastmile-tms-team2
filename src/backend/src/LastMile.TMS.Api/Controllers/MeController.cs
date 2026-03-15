using LastMile.TMS.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LastMile.TMS.Api.Controllers;

/// <summary>
/// Minimal endpoint that returns the current authenticated user's info.
/// Used to verify JWT authentication is working end-to-end.
/// </summary>
[ApiController]
[Route("api/me")]
[Authorize]
public class MeController(ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    public IActionResult GetCurrentUser()
    {
        return Ok(new
        {
            userId = currentUser.UserId,
            userName = currentUser.UserName
        });
    }
}
