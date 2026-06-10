using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/tenancy")]
public class TenancyController(ITenantContext tenantContext, AppDbContext dbContext) : ControllerBase
{
    [HttpGet("context")]
    public IActionResult GetContext() =>
        Ok(new
        {
            tenantId = tenantContext.TenantId,
            isPlatformAdmin = tenantContext.IsPlatformAdmin,
            hasTenant = tenantContext.HasTenant
        });

    [HttpGet("branches")]
    public async Task<IActionResult> GetBranches(CancellationToken cancellationToken)
    {
        var branches = await dbContext.Branches
            .AsNoTracking()
            .OrderBy(b => b.Name)
            .Select(b => new { b.Id, b.Code, b.Name, b.TenantId })
            .ToListAsync(cancellationToken);

        return Ok(branches);
    }
}
