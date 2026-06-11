using Microsoft.AspNetCore.Authorization;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Api.Controllers;

[Authorize(Policy = AuthorizationPolicies.ReportsRead)]
public abstract class ReportsApiControllerBase : AuthorizedApiControllerBase;
