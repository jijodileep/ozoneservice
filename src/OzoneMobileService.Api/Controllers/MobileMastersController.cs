using MediatR;
using Microsoft.AspNetCore.Mvc;
using OzoneMobileService.Application.DTOs.Common;
using OzoneMobileService.Application.DTOs.MobileMasters;
using OzoneMobileService.Application.Features.MobileMasters.Queries;
using OzoneMobileService.Application.Interfaces;

namespace OzoneMobileService.Api.Controllers;

/// <summary>
/// Read-only mobile catalog shared across all tenants.
/// </summary>
[Route("api/mobile-masters")]
public class MobileMastersController(IMediator mediator, ITenantContext tenantContext)
    : TenantApiControllerBase(tenantContext)
{
    [HttpGet("brands")]
    [ProducesResponseType(typeof(PagedResult<MobileBrandResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBrands(
        [FromQuery] bool activeOnly,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (RequireTenant(out _) is { } error)
        {
            return error;
        }

        var brands = await mediator.Send(
            new GetMobileBrandsPagedQuery(activeOnly, search, page, pageSize),
            cancellationToken);

        return Ok(brands);
    }

    [HttpGet("brands/{brandId:guid}/models")]
    [ProducesResponseType(typeof(IReadOnlyList<MobileModelResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetModels(
        Guid brandId,
        [FromQuery] bool activeOnly,
        CancellationToken cancellationToken)
    {
        if (RequireTenant(out _) is { } error)
        {
            return error;
        }

        var models = await mediator.Send(new GetMobileModelsQuery(brandId, activeOnly), cancellationToken);
        return Ok(models);
    }

    [HttpGet("models/{modelId:guid}/variants")]
    [ProducesResponseType(typeof(IReadOnlyList<MobileVariantResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVariants(
        Guid modelId,
        [FromQuery] bool activeOnly,
        CancellationToken cancellationToken)
    {
        if (RequireTenant(out _) is { } error)
        {
            return error;
        }

        var variants = await mediator.Send(new GetMobileVariantsQuery(modelId, activeOnly), cancellationToken);
        return Ok(variants);
    }
}
