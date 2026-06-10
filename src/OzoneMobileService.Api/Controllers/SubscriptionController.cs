using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OzoneMobileService.Application.DTOs.Subscription;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Api.Controllers;

[Authorize(Policy = AuthorizationPolicies.SetupWrite)]
[ApiController]
[Route("api/subscription")]
public class SubscriptionController(
    ISubscriptionService subscriptionService,
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

        var options = await subscriptionService.GetOptionsAsync(
            tenantContext.TenantId!.Value,
            cancellationToken);

        return options is null ? NotFound() : Ok(options);
    }

    [HttpPost("upgrade")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Upgrade(
        [FromBody] UpgradePlanRequest request,
        CancellationToken cancellationToken)
    {
        if (!tenantContext.HasTenant)
        {
            return BadRequest(new { message = "Tenant context required." });
        }

        var ok = await subscriptionService.UpgradePlanAsync(
            tenantContext.TenantId!.Value,
            request.PlanId,
            cancellationToken);

        return ok ? NoContent() : BadRequest(new { message = "Invalid plan." });
    }
}
