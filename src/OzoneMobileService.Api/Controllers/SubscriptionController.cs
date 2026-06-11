using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OzoneMobileService.Application.DTOs.Subscription;
using OzoneMobileService.Application.Exceptions;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Api.Controllers;

[Authorize(Policy = AuthorizationPolicies.SetupWrite)]
[ApiController]
[Route("api/subscription")]
public class SubscriptionController(
    ISubscriptionService subscriptionService,
    IUpgradeRequestService upgradeRequestService,
    ITenantContext tenantContext) : ControllerBase
{
    [HttpGet("options")]
    [ProducesResponseType(typeof(SubscriptionOptionsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOptions(CancellationToken cancellationToken)
    {
        if (!tenantContext.HasTenant)
        {
            return BadRequest(new { message = "Tenant context required." });
        }

        var options = await subscriptionService.GetOptionsWithPendingAsync(
            tenantContext.TenantId!.Value,
            cancellationToken);

        return options is null ? NotFound() : Ok(options);
    }

    [HttpGet("upgrade-requests")]
    [ProducesResponseType(typeof(IReadOnlyList<UpgradeRequestResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUpgradeRequests(CancellationToken cancellationToken)
    {
        if (!tenantContext.HasTenant)
        {
            return BadRequest(new { message = "Tenant context required." });
        }

        var items = await upgradeRequestService.GetTenantRequestsAsync(
            tenantContext.TenantId!.Value,
            cancellationToken);

        return Ok(items);
    }

    [HttpPost("upgrade-request")]
    [ProducesResponseType(typeof(UpgradeRequestResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RequestUpgrade(
        [FromBody] UpgradePlanRequest request,
        CancellationToken cancellationToken)
    {
        if (!tenantContext.HasTenant)
        {
            return BadRequest(new { message = "Tenant context required." });
        }

        try
        {
            var result = await upgradeRequestService.RequestUpgradeAsync(
                tenantContext.TenantId!.Value,
                request.PlanId,
                cancellationToken);

            return result is null
                ? BadRequest(new { message = "Invalid plan." })
                : CreatedAtAction(nameof(GetUpgradeRequests), result);
        }
        catch (PlanUpgradeException)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only higher-tier plans can be requested." });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
