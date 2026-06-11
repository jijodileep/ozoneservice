using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OzoneMobileService.Application.DTOs.Auth;
using OzoneMobileService.Application.Features.Auth.Commands;
using OzoneMobileService.Application.Features.Auth.Queries;

namespace OzoneMobileService.Api.Controllers;

[Route("api/auth")]
public class AuthController(IMediator mediator) : AuthorizedApiControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var tokens = await mediator.Send(new LoginCommand(request), cancellationToken);
        return tokens is null
            ? Unauthorized(new { message = "Invalid email or password." })
            : Ok(tokens);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var tokens = await mediator.Send(new RefreshTokenCommand(request), cancellationToken);
        return tokens is null
            ? Unauthorized(new { message = "Invalid or expired refresh token." })
            : Ok(tokens);
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var profile = await mediator.Send(new GetUserProfileQuery(userId), cancellationToken);
        return profile is null ? NotFound() : Ok(profile);
    }
}
