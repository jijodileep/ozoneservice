using MediatR;
using Microsoft.AspNetCore.Mvc;
using OzoneMobileService.Application.DTOs.Branches;
using OzoneMobileService.Application.Features.Branches.Commands;
using OzoneMobileService.Application.Features.Branches.Queries;
using OzoneMobileService.Application.Interfaces;

namespace OzoneMobileService.Api.Controllers;

[Route("api/branches")]
public class BranchesController(IMediator mediator, ITenantContext tenantContext)
    : SetupApiControllerBase(tenantContext)
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<BranchResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBranches(CancellationToken cancellationToken)
    {
        if (RequireTenantAndUser(out _, out var userId) is { } error)
        {
            return error;
        }

        var branches = await mediator.Send(new GetBranchesQuery(userId), cancellationToken);
        return Ok(branches);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BranchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBranch(Guid id, CancellationToken cancellationToken)
    {
        if (RequireTenantAndUser(out _, out var userId) is { } error)
        {
            return error;
        }

        var branch = await mediator.Send(new GetBranchQuery(id, userId), cancellationToken);
        return branch is null ? NotFound() : Ok(branch);
    }

    [HttpPost]
    [ProducesResponseType(typeof(BranchResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateBranch(
        [FromBody] CreateBranchRequest request,
        CancellationToken cancellationToken)
    {
        if (RequireTenantAndUser(out var tenantId, out var userId) is { } error)
        {
            return error;
        }

        var branch = await mediator.Send(
            new CreateBranchCommand(
                tenantId,
                userId,
                request.Code,
                request.Name,
                request.Address,
                request.Phone,
                request.GstNumber),
            cancellationToken);

        if (branch is null)
        {
            return BadRequestMessage("Branch code already exists or you are not allowed to create branches.");
        }

        return CreatedAtAction(nameof(GetBranch), new { id = branch.Id }, branch);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(BranchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBranch(
        Guid id,
        [FromBody] UpdateBranchRequest request,
        CancellationToken cancellationToken)
    {
        if (RequireTenantAndUser(out _, out var userId) is { } error)
        {
            return error;
        }

        var branch = await mediator.Send(
            new UpdateBranchCommand(
                id,
                userId,
                request.Name,
                request.Address,
                request.Phone,
                request.GstNumber,
                request.IsActive),
            cancellationToken);

        return branch is null ? NotFound() : Ok(branch);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeactivateBranch(Guid id, CancellationToken cancellationToken)
    {
        if (RequireTenantAndUser(out _, out var userId) is { } error)
        {
            return error;
        }

        var deactivated = await mediator.Send(new DeactivateBranchCommand(id, userId), cancellationToken);
        return deactivated ? NoContent() : NotFound();
    }
}
