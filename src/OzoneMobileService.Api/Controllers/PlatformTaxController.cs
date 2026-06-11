using MediatR;
using Microsoft.AspNetCore.Mvc;
using OzoneMobileService.Application.DTOs.Platform;
using OzoneMobileService.Application.Features.Platform.Commands;
using OzoneMobileService.Application.Features.Platform.Queries;

namespace OzoneMobileService.Api.Controllers;

[Route("api/platform/tax")]
public class PlatformTaxController(IMediator mediator) : PlatformApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(TaxConfigurationResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var config = await mediator.Send(new GetTaxConfigurationQuery(), cancellationToken);
        return Ok(config);
    }

    [HttpPut]
    [ProducesResponseType(typeof(TaxConfigurationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        [FromBody] UpdateTaxConfigurationRequest request,
        CancellationToken cancellationToken)
    {
        var config = await mediator.Send(new UpdateTaxConfigurationCommand(request), cancellationToken);
        return config is null
            ? BadRequest(new { message = "Invalid tax configuration." })
            : Ok(config);
    }
}
