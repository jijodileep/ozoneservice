using MediatR;
using Microsoft.AspNetCore.Mvc;
using OzoneMobileService.Application.DTOs.Common;
using OzoneMobileService.Application.DTOs.Subscription;
using OzoneMobileService.Application.Features.UpgradeRequests.Commands;
using OzoneMobileService.Application.Features.UpgradeRequests.Queries;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Api.Controllers;

[Route("api/platform/upgrade-requests")]
public class PlatformUpgradeRequestsController(IMediator mediator) : PlatformApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UpgradeRequestResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRequests(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = UpgradeRequestStatuses.Pending,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetUpgradeRequestsPagedQuery(page, pageSize, status),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id:guid}/approve")]
    [ProducesResponseType(typeof(UpgradeRequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var reviewerId))
        {
            return Unauthorized();
        }

        var result = await mediator.Send(new ApproveUpgradeRequestCommand(id, reviewerId), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{id:guid}/reject")]
    [ProducesResponseType(typeof(UpgradeRequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reject(
        Guid id,
        [FromBody] RejectUpgradeRequestRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var reviewerId))
        {
            return Unauthorized();
        }

        var result = await mediator.Send(
            new RejectUpgradeRequestCommand(id, reviewerId, request.Reason),
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }
}
