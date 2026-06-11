using MediatR;
using Microsoft.AspNetCore.Mvc;
using OzoneMobileService.Application.DTOs.Platform;
using OzoneMobileService.Application.Features.Platform.Commands;
using OzoneMobileService.Application.Features.Platform.Queries;

namespace OzoneMobileService.Api.Controllers;

[Route("api/platform/plans")]
public class PlatformPlansController(IMediator mediator) : PlatformApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SubscriptionPlanResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlans(CancellationToken cancellationToken)
    {
        var plans = await mediator.Send(new GetPlansQuery(), cancellationToken);
        return Ok(plans);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SubscriptionPlanResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePlan(
        [FromBody] CreateSubscriptionPlanRequest request,
        CancellationToken cancellationToken)
    {
        var plan = await mediator.Send(new CreatePlanCommand(request), cancellationToken);
        return plan is null
            ? BadRequest(new { message = "Plan code already exists." })
            : CreatedAtAction(nameof(GetPlans), plan);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SubscriptionPlanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePlan(
        Guid id,
        [FromBody] UpdateSubscriptionPlanRequest request,
        CancellationToken cancellationToken)
    {
        var plan = await mediator.Send(new UpdatePlanCommand(id, request), cancellationToken);
        return plan is null ? NotFound() : Ok(plan);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeletePlan(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await mediator.Send(new DeletePlanCommand(id), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
