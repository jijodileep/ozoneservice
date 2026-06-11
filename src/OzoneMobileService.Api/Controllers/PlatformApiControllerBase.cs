using Microsoft.AspNetCore.Authorization;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Api.Controllers;

[Authorize(Policy = AuthorizationPolicies.PlatformSuperAdmin)]
public abstract class PlatformApiControllerBase : AuthorizedApiControllerBase;
