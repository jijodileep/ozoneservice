using MediatR;
using Microsoft.AspNetCore.Mvc;
using OzoneMobileService.Application.DTOs.Subscription;
using OzoneMobileService.Application.Exceptions;
using OzoneMobileService.Application.Features.Subscription.Queries;
using OzoneMobileService.Application.Features.UpgradeRequests.Commands;
using OzoneMobileService.Application.Features.UpgradeRequests.Queries;
using OzoneMobileService.Application.Interfaces;

namespace OzoneMobileService.Api.Controllers;

[Route("api/subscription")]
public class SubscriptionController(IMediator mediator, ITenantContext tenantContext)
    : SetupApiControllerBase(tenantContext)
{
    [HttpGet("options")]
    [ProducesResponseType(typeof(SubscriptionOptionsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOptions(CancellationToken cancellationToken)
    {
        if (RequireTenant(out var tenantId) is { } error)
        {
            return error;
        }

        var options = await mediator.Send(new GetSubscriptionOptionsQuery(tenantId), cancellationToken);
        return options is null ? NotFound() : Ok(options);
    }

    [HttpGet("upgrade-requests")]
    [ProducesResponseType(typeof(IReadOnlyList<UpgradeRequestResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUpgradeRequests(CancellationToken cancellationToken)
    {
        if (RequireTenant(out var tenantId) is { } error)
        {
            return error;
        }

        var items = await mediator.Send(new GetTenantUpgradeRequestsQuery(tenantId), cancellationToken);
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
        if (RequireTenant(out var tenantId) is { } error)
        {
            return error;
        }

        try
        {
            var result = await mediator.Send(
                new RequestUpgradeCommand(tenantId, request.PlanId),
                cancellationToken);

            return result is null
                ? BadRequestMessage("Invalid plan.")
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
