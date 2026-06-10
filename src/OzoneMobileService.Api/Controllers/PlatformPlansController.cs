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
}
