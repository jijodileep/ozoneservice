using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OzoneMobileService.Application.DTOs.Common;
using OzoneMobileService.Application.DTOs.Platform;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Api.Controllers;

[ApiController]
[Route("api/platform/shops")]
[Authorize(Policy = AuthorizationPolicies.PlatformSuperAdmin)]
public class PlatformShopsController(IPlatformService platformService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ShopResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetShops(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var shops = await platformService.GetShopsPagedAsync(page, pageSize, search, cancellationToken);
        return Ok(shops);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ShopResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateShop(
        [FromBody] CreateShopRequest request,
        CancellationToken cancellationToken)
    {
        var shop = await platformService.CreateShopAsync(request, cancellationToken);
        if (shop is null)
        {
            return BadRequest(new { message = "Could not create shop. Check plan id, unique code, and admin email." });
        }

        return CreatedAtAction(nameof(GetShops), new { id = shop.Id }, shop);
    }

    [HttpPatch("{id:guid}/suspend")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Suspend(Guid id, CancellationToken cancellationToken)
    {
        var ok = await platformService.SuspendShopAsync(id, cancellationToken);
        return ok ? NoContent() : NotFound();
    }

    [HttpPatch("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        var ok = await platformService.ActivateShopAsync(id, cancellationToken);
        return ok ? NoContent() : NotFound();
    }

    [HttpPost("{id:guid}/assign-plan")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignPlan(
        Guid id,
        [FromBody] AssignPlanRequest request,
        CancellationToken cancellationToken)
    {
        var ok = await platformService.AssignPlanAsync(id, request.SubscriptionPlanId, cancellationToken);
        return ok ? NoContent() : NotFound();
    }
}

public sealed record AssignPlanRequest(Guid SubscriptionPlanId);
