using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Api.Controllers;

/// <summary>Dev-only policy verification endpoints. Remove or lock down before production.</summary>
[Authorize]
[ApiController]
[Route("api/authorization-test")]
public class AuthorizationTestController(IBranchContext branchContext) : ControllerBase
{
    [Authorize(Policy = AuthorizationPolicies.SetupWrite)]
    [HttpPost("setup")]
    public IActionResult SetupWrite() =>
        Ok(new { message = "Setup write allowed.", branchId = branchContext.BranchId });

    [Authorize(Policy = AuthorizationPolicies.OperationalWrite)]
    [HttpPost("operational")]
    public IActionResult OperationalWrite() =>
        Ok(new { message = "Operational write allowed.", branchId = branchContext.BranchId });

    [Authorize(Policy = AuthorizationPolicies.ReportsRead)]
    [HttpGet("reports")]
    public IActionResult ReportsRead() =>
        Ok(new { message = "Reports read allowed.", branchId = branchContext.BranchId });

    [HttpGet("branch")]
    public IActionResult GetBranch() =>
        Ok(new { branchId = branchContext.BranchId, hasBranch = branchContext.HasBranch });
}
