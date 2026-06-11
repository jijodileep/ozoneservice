using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OzoneMobileService.Application.DTOs.Customers;
using OzoneMobileService.Application.Exceptions;
using OzoneMobileService.Application.Features.Customers.Commands;
using OzoneMobileService.Application.Features.Customers.Queries;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Api.Controllers;

[Route("api/customers")]
public class CustomersController(
    IMediator mediator,
    ITenantContext tenantContext,
    IBranchContext branchContext)
    : TenantApiControllerBase(tenantContext)
{
    [HttpGet("lookup")]
    [ProducesResponseType(typeof(CustomerDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LookupCustomer(
        [FromQuery] string mobile,
        CancellationToken cancellationToken)
    {
        if (RequireTenant(out var tenantId) is { } error)
        {
            return error;
        }

        var customer = await mediator.Send(new LookupCustomerQuery(tenantId, mobile), cancellationToken);
        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomer(Guid id, CancellationToken cancellationToken)
    {
        if (RequireTenant(out var tenantId) is { } error)
        {
            return error;
        }

        var customer = await mediator.Send(new GetCustomerQuery(tenantId, id), cancellationToken);
        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.OperationalWrite)]
    [ProducesResponseType(typeof(CustomerDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateCustomer(
        [FromBody] CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        if (RequireTenant(out var tenantId) is { } error)
        {
            return error;
        }

        try
        {
            var customer = await mediator.Send(
                new CreateCustomerCommand(
                    tenantId,
                    request.Name,
                    request.MobileNumber,
                    request.Email,
                    request.Address),
                cancellationToken);

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }
        catch (CustomerDuplicateException ex)
        {
            return Conflict(new { message = ex.Message, mobileNumber = ex.MobileNumber });
        }
    }

    [HttpPost("{id:guid}/devices")]
    [Authorize(Policy = AuthorizationPolicies.OperationalWrite)]
    [ProducesResponseType(typeof(CustomerDeviceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddDevice(
        Guid id,
        [FromBody] AddCustomerDeviceRequest request,
        CancellationToken cancellationToken)
    {
        if (RequireTenant(out var tenantId) is { } error)
        {
            return error;
        }

        if (!branchContext.HasBranch)
        {
            return BadRequestMessage("Branch context required.");
        }

        var device = await mediator.Send(
            new AddCustomerDeviceCommand(
                tenantId,
                id,
                branchContext.BranchId!.Value,
                request.VariantId,
                request.Imei),
            cancellationToken);

        return device is null ? NotFound() : CreatedAtAction(nameof(GetCustomer), new { id }, device);
    }
}
