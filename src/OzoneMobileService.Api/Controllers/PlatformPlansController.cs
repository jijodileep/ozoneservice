using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OzoneMobileService.Application.DTOs.Platform;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Api.Controllers;

[ApiController]
[Route("api/platform/plans")]
[Authorize(Policy = AuthorizationPolicies.PlatformSuperAdmin)]
public class PlatformPlansController(IPlatformService platformService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SubscriptionPlanResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlans(CancellationToken cancellationToken)
    {
        var plans = await platformService.GetPlansAsync(cancellationToken);
        return Ok(plans);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SubscriptionPlanResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePlan(
        [FromBody] CreateSubscriptionPlanRequest request,
        CancellationToken cancellationToken)
    {
        var plan = await platformService.CreatePlanAsync(request, cancellationToken);
        if (plan is null)
        {
            return BadRequest(new { message = "Plan code already exists." });
        }

        return CreatedAtAction(nameof(GetPlans), plan);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SubscriptionPlanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePlan(
        Guid id,
        [FromBody] UpdateSubscriptionPlanRequest request,
        CancellationToken cancellationToken)
    {
        var plan = await platformService.UpdatePlanAsync(id, request, cancellationToken);
        return plan is null ? NotFound() : Ok(plan);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeletePlan(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await platformService.DeletePlanAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
