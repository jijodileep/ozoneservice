using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Infrastructure.MultiTenancy;

public class TenantContext(IHttpContextAccessor httpContextAccessor) : ITenantContext
{
    public Guid? TenantId
    {
        get
        {
            var claim = httpContextAccessor.HttpContext?.User?.FindFirstValue("tenant_id");
            if (string.IsNullOrWhiteSpace(claim))
            {
                return null;
            }

            return Guid.TryParse(claim, out var tenantId) ? tenantId : null;
        }
    }

    public bool IsPlatformAdmin =>
        httpContextAccessor.HttpContext?.User?.IsInRole(Roles.PlatformSuperAdmin) == true;
}
