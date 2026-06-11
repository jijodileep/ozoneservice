using MediatR;
using Microsoft.AspNetCore.Mvc;
using OzoneMobileService.Application.DTOs.Users;
using OzoneMobileService.Application.Features.Users.Commands;
using OzoneMobileService.Application.Features.Users.Queries;
using OzoneMobileService.Application.Interfaces;

namespace OzoneMobileService.Api.Controllers;

[Route("api/users")]
public class UsersController(IMediator mediator, ITenantContext tenantContext)
    : SetupApiControllerBase(tenantContext)
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UserResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
    {
        if (RequireTenantAndUser(out var tenantId, out var userId) is { } error)
        {
            return error;
        }

        var users = await mediator.Send(new GetUsersQuery(tenantId, userId), cancellationToken);
        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser(Guid id, CancellationToken cancellationToken)
    {
        if (RequireTenantAndUser(out var tenantId, out var userId) is { } error)
        {
            return error;
        }

        var user = await mediator.Send(new GetUserQuery(id, tenantId, userId), cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        if (RequireTenantAndUser(out var tenantId, out var userId) is { } error)
        {
            return error;
        }

        var user = await mediator.Send(
            new CreateUserCommand(
                tenantId,
                userId,
                request.Email,
                request.DisplayName,
                request.Password,
                request.Role,
                request.BranchIds),
            cancellationToken);

        if (user is null)
        {
            return BadRequestMessage(
                "Could not create user. Check email uniqueness, branch assignment, and your permissions.");
        }

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(
        Guid id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        if (RequireTenantAndUser(out var tenantId, out var userId) is { } error)
        {
            return error;
        }

        var user = await mediator.Send(
            new UpdateUserCommand(
                id,
                tenantId,
                userId,
                request.DisplayName,
                request.Role,
                request.BranchIds,
                request.IsActive),
            cancellationToken);

        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost("{id:guid}/reset-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetUserPassword(
        Guid id,
        [FromBody] ResetUserPasswordRequest request,
        CancellationToken cancellationToken)
    {
        if (RequireTenantAndUser(out var tenantId, out var userId) is { } error)
        {
            return error;
        }

        var reset = await mediator.Send(
            new ResetUserPasswordCommand(id, tenantId, userId, request.NewPassword),
            cancellationToken);

        return reset ? NoContent() : NotFound();
    }
}
