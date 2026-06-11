using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OzoneMobileService.Application.DTOs.Common;
using OzoneMobileService.Application.DTOs.Subscription;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Api.Controllers;

[ApiController]
[Route("api/platform/upgrade-requests")]
[Authorize(Policy = AuthorizationPolicies.PlatformSuperAdmin)]
public class PlatformUpgradeRequestsController(IUpgradeRequestService upgradeRequestService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UpgradeRequestResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRequests(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = UpgradeRequestStatuses.Pending,
        CancellationToken cancellationToken = default)
    {
        var result = await upgradeRequestService.GetPendingRequestsPagedAsync(
            page,
            pageSize,
            status,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id:guid}/approve")]
    [ProducesResponseType(typeof(UpgradeRequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        var reviewerId = GetUserId();
        if (reviewerId is null)
        {
            return Unauthorized();
        }

        var result = await upgradeRequestService.ApproveRequestAsync(id, reviewerId.Value, cancellationToken);
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
        var reviewerId = GetUserId();
        if (reviewerId is null)
        {
            return Unauthorized();
        }

        var result = await upgradeRequestService.RejectRequestAsync(
            id,
            reviewerId.Value,
            request.Reason,
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    private Guid? GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return claim is not null && Guid.TryParse(claim, out var id) ? id : null;
    }
}
