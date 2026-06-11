using Microsoft.AspNetCore.Mvc;
using OzoneMobileService.Application.Interfaces;

namespace OzoneMobileService.Api.Controllers;

public abstract class TenantApiControllerBase(ITenantContext tenantContext) : AuthorizedApiControllerBase
{
    protected ITenantContext TenantContext { get; } = tenantContext;

    protected IActionResult? RequireTenant(out Guid tenantId)
    {
        if (!TenantContext.HasTenant)
        {
            tenantId = default;
            return BadRequestMessage("Tenant context required.");
        }

        tenantId = TenantContext.TenantId!.Value;
        return null;
    }

    protected IActionResult? RequireTenantAndUser(out Guid tenantId, out Guid userId)
    {
        if (!TryGetUserId(out userId) || !TenantContext.HasTenant)
        {
            tenantId = default;
            userId = default;
            return BadRequestMessage("Tenant context required.");
        }

        tenantId = TenantContext.TenantId!.Value;
        return null;
    }
}
