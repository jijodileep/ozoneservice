using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OzoneMobileService.Application.DTOs.MobileMasters;
using OzoneMobileService.Application.Features.MobileMasters.Commands;
using OzoneMobileService.Application.Features.MobileMasters.Queries;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Api.Controllers;

[Route("api/mobile-masters")]
public class MobileMastersController(IMediator mediator, ITenantContext tenantContext)
    : TenantApiControllerBase(tenantContext)
{
    [HttpGet("brands")]
    [ProducesResponseType(typeof(IReadOnlyList<MobileBrandResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBrands(
        [FromQuery] bool activeOnly,
        CancellationToken cancellationToken)
    {
        if (RequireTenant(out _) is { } error)
        {
            return error;
        }

        var brands = await mediator.Send(new GetMobileBrandsQuery(activeOnly), cancellationToken);
        return Ok(brands);
    }

    [HttpPost("brands")]
    [Authorize(Policy = AuthorizationPolicies.SetupWrite)]
    [ProducesResponseType(typeof(MobileBrandResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBrand(
        [FromBody] CreateMobileBrandRequest request,
        CancellationToken cancellationToken)
    {
        if (RequireTenant(out var tenantId) is { } error)
        {
            return error;
        }

        var brand = await mediator.Send(
            new CreateMobileBrandCommand(tenantId, request.Name),
            cancellationToken);

        return brand is null
            ? BadRequestMessage("Brand name already exists.")
            : CreatedAtAction(nameof(GetBrands), brand);
    }

    [HttpPut("brands/{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.SetupWrite)]
    [ProducesResponseType(typeof(MobileBrandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBrand(
        Guid id,
        [FromBody] UpdateMobileBrandRequest request,
        CancellationToken cancellationToken)
    {
        if (RequireTenant(out _) is { } error)
        {
            return error;
        }

        var brand = await mediator.Send(
            new UpdateMobileBrandCommand(id, request.Name, request.IsActive),
            cancellationToken);

        return brand is null ? NotFound() : Ok(brand);
    }

    [HttpDelete("brands/{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.SetupWrite)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateBrand(Guid id, CancellationToken cancellationToken)
    {
        if (RequireTenant(out _) is { } error)
        {
            return error;
        }

        var deactivated = await mediator.Send(new DeactivateMobileBrandCommand(id), cancellationToken);
        return deactivated ? NoContent() : NotFound();
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

    [HttpPost("brands/{brandId:guid}/models")]
    [Authorize(Policy = AuthorizationPolicies.SetupWrite)]
    [ProducesResponseType(typeof(MobileModelResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateModel(
        Guid brandId,
        [FromBody] CreateMobileModelRequest request,
        CancellationToken cancellationToken)
    {
        if (RequireTenant(out var tenantId) is { } error)
        {
            return error;
        }

        var model = await mediator.Send(
            new CreateMobileModelCommand(tenantId, brandId, request.Name),
            cancellationToken);

        return model is null
            ? BadRequestMessage("Model name already exists or brand was not found.")
            : CreatedAtAction(nameof(GetModels), new { brandId }, model);
    }

    [HttpPut("models/{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.SetupWrite)]
    [ProducesResponseType(typeof(MobileModelResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateModel(
        Guid id,
        [FromBody] UpdateMobileModelRequest request,
        CancellationToken cancellationToken)
    {
        if (RequireTenant(out _) is { } error)
        {
            return error;
        }

        var model = await mediator.Send(
            new UpdateMobileModelCommand(id, request.Name, request.IsActive),
            cancellationToken);

        return model is null ? NotFound() : Ok(model);
    }

    [HttpDelete("models/{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.SetupWrite)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateModel(Guid id, CancellationToken cancellationToken)
    {
        if (RequireTenant(out _) is { } error)
        {
            return error;
        }

        var deactivated = await mediator.Send(new DeactivateMobileModelCommand(id), cancellationToken);
        return deactivated ? NoContent() : NotFound();
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

    [HttpPost("models/{modelId:guid}/variants")]
    [Authorize(Policy = AuthorizationPolicies.SetupWrite)]
    [ProducesResponseType(typeof(MobileVariantResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateVariant(
        Guid modelId,
        [FromBody] CreateMobileVariantRequest request,
        CancellationToken cancellationToken)
    {
        if (RequireTenant(out var tenantId) is { } error)
        {
            return error;
        }

        var variant = await mediator.Send(
            new CreateMobileVariantCommand(tenantId, modelId, request.Name),
            cancellationToken);

        return variant is null
            ? BadRequestMessage("Variant name already exists or model was not found.")
            : CreatedAtAction(nameof(GetVariants), new { modelId }, variant);
    }

    [HttpPut("variants/{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.SetupWrite)]
    [ProducesResponseType(typeof(MobileVariantResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVariant(
        Guid id,
        [FromBody] UpdateMobileVariantRequest request,
        CancellationToken cancellationToken)
    {
        if (RequireTenant(out _) is { } error)
        {
            return error;
        }

        var variant = await mediator.Send(
            new UpdateMobileVariantCommand(id, request.Name, request.IsActive),
            cancellationToken);

        return variant is null ? NotFound() : Ok(variant);
    }

    [HttpDelete("variants/{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.SetupWrite)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateVariant(Guid id, CancellationToken cancellationToken)
    {
        if (RequireTenant(out _) is { } error)
        {
            return error;
        }

        var deactivated = await mediator.Send(new DeactivateMobileVariantCommand(id), cancellationToken);
        return deactivated ? NoContent() : NotFound();
    }
}
