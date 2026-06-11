using Microsoft.AspNetCore.Authorization;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Api.Controllers;

[Authorize(Policy = AuthorizationPolicies.SetupWrite)]
public abstract class SetupApiControllerBase(ITenantContext tenantContext) : TenantApiControllerBase(tenantContext);
