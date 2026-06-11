using MediatR;
using Microsoft.AspNetCore.Mvc;
using OzoneMobileService.Application.Features.Branches.Queries;
using OzoneMobileService.Application.Interfaces;

namespace OzoneMobileService.Api.Controllers;

[Route("api/tenancy")]
public class TenancyController(IMediator mediator, ITenantContext tenantContext)
    : TenantApiControllerBase(tenantContext)
{
    [HttpGet("context")]
    public IActionResult GetContext() =>
        Ok(new
        {
            tenantId = TenantContext.TenantId,
            isPlatformAdmin = TenantContext.IsPlatformAdmin,
            hasTenant = TenantContext.HasTenant
        });

    [HttpGet("branches")]
    public async Task<IActionResult> GetBranches(CancellationToken cancellationToken)
    {
        if (RequireTenantAndUser(out _, out var userId) is { } error)
        {
            return error;
        }

        var branches = await mediator.Send(new GetBranchesQuery(userId), cancellationToken);

        return Ok(branches
            .Where(b => b.IsActive)
            .Select(b => new { b.Id, b.Code, b.Name, b.TenantId }));
    }
}
