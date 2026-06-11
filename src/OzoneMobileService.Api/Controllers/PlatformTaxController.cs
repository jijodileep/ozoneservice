using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OzoneMobileService.Application.DTOs.Platform;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Api.Controllers;

[ApiController]
[Route("api/platform/tax")]
[Authorize(Policy = AuthorizationPolicies.PlatformSuperAdmin)]
public class PlatformTaxController(ITaxService taxService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(TaxConfigurationResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var config = await taxService.GetConfigurationAsync(cancellationToken);
        return Ok(config);
    }

    [HttpPut]
    [ProducesResponseType(typeof(TaxConfigurationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        [FromBody] UpdateTaxConfigurationRequest request,
        CancellationToken cancellationToken)
    {
        var config = await taxService.UpdateConfigurationAsync(request, cancellationToken);
        return config is null
            ? BadRequest(new { message = "Invalid tax configuration." })
            : Ok(config);
    }
}
