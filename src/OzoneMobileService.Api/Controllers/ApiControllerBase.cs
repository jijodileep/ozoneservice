using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace OzoneMobileService.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected bool TryGetUserId(out Guid userId)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(claim, out userId);
    }

    protected Guid? GetUserId() =>
        TryGetUserId(out var userId) ? userId : null;

    protected IActionResult BadRequestMessage(string message) =>
        BadRequest(new { message });
}
