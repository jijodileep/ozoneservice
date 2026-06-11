using Microsoft.AspNetCore.Authorization;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Api.Controllers;

[Authorize(Policy = AuthorizationPolicies.OperationalWrite)]
public abstract class OperationalApiControllerBase(ITenantContext tenantContext)
    : TenantApiControllerBase(tenantContext);
